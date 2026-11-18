using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace IronDomeX
{
    // Visual: o projetil falso some quando o silo dispara e volta subindo pelo poco.
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_SmallMissileLauncher), false, "IronDomeX")]
    public class IronDomeXFake : MyGameLogicComponent
    {
        public override void Close()
        {
            if (_manual && _fake != null) _fake.Close();
        }

        const float HiddenSeconds = 2.5f;   // tempo escondido depois do disparo
        const float RiseSeconds = 1.5f;     // tempo subindo de volta
        const float RiseDepth = 0.55f;      // quanto ele comeca abaixo do lugar (m)

        IMySmallMissileLauncher _block;
        MyEntity _fake;
        bool _manual;
        int _frames;
        static readonly Vector3 FakeOffset = new Vector3(0f, 0f, 1.054f);   // posicao do encaixe no modelo
        Matrix _home;
        float _timer = -1f;                 // -1 = parado no lugar

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            _block = (IMySmallMissileLauncher)Entity;
            if (!MyAPIGateway.Utilities.IsDedicated)
                NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
        }

        MyEntity Fake()
        {
            var entity = (MyEntity)_block;
            if (entity.Subparts != null)
            {
                foreach (var pair in entity.Subparts)
                {
                    if (pair.Key.IndexOf("Fake", StringComparison.OrdinalIgnoreCase) < 0) continue;
                    if (pair.Value != _fake)
                    {
                        if (_manual && _fake != null) _fake.Close();
                        _manual = false;
                        _fake = pair.Value;
                        _home = Matrix.CreateTranslation(FakeOffset);
                    }
                    return _fake;
                }
            }

            if (_fake != null && !_fake.Closed && !_fake.MarkedForClose) return _fake;

            // o jogo nao criou a peca neste tipo de bloco: cria como filho do bloco
            if (++_frames < 60 || !_block.InScene || _block.CubeGrid.Physics == null) return null;
            var model = IronDomeXSession.ModPath + "\\Models\\IronDomeX\\IronDomeX_Fake.mwm";
            var fake = new MyEntity();
            fake.Init(null, model, (MyEntity)_block, 1f);
            fake.Save = false;
            fake.SyncFlag = false;
            _home = Matrix.CreateTranslation(FakeOffset);
            fake.PositionComp.SetLocalMatrix(ref _home);
            MyAPIGateway.Entities.AddEntity(fake);
            _fake = fake;
            _manual = true;
            return _fake;
        }

        public void OnShot()
        {
            _timer = 0f;
            var fake = Fake();
            if (fake != null) fake.Render.Visible = false;
        }

        public override void UpdateAfterSimulation()
        {
            if (_fake == null || _fake.Closed)
            {
                try { Fake(); } catch (Exception e) { MyLog.Default.WriteLineAndConsole("[IronDomeX] " + e); }
            }
            if (_timer < 0f) return;
            try
            {
                var fake = Fake();
                if (fake == null) return;

                _timer += 1f / 60f;
                if (_timer < HiddenSeconds)
                {
                    fake.Render.Visible = false;
                    return;
                }

                float t = Math.Min(1f, (_timer - HiddenSeconds) / RiseSeconds);
                t = t * t * (3f - 2f * t);                  // suaviza inicio e fim
                var matrix = _home;
                matrix.Translation += Vector3.Backward * (RiseDepth * (1f - t));
                fake.PositionComp.SetLocalMatrix(ref matrix);
                fake.Render.Visible = true;

                if (t >= 1f) _timer = -1f;
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLineAndConsole("[IronDomeX] " + e);
            }
        }
    }

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class IronDomeXSession : MySessionComponentBase
    {
        public static string ModPath;

        public override void LoadData()
        {
            ModPath = ModContext.ModPath;
        }

        public override void BeforeStart()
        {
            if (MyAPIGateway.Utilities.IsDedicated || MyAPIGateway.Missiles == null) return;
            MyAPIGateway.Missiles.OnMissileAdded += OnMissileAdded;
        }

        protected override void UnloadData()
        {
            if (MyAPIGateway.Missiles != null)
                MyAPIGateway.Missiles.OnMissileAdded -= OnMissileAdded;
        }

        void OnMissileAdded(IMyMissile missile)
        {
            try
            {
                var block = MyAPIGateway.Entities.GetEntityById(missile.LauncherId) as IMySmallMissileLauncher;
                if (block == null || block.GameLogic == null) return;
                var logic = block.GameLogic.GetAs<IronDomeXFake>();
                if (logic != null) logic.OnShot();
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLineAndConsole("[IronDomeX] " + e);
            }
        }
    }
}
