using System;

namespace Linkhub.BouncyCastle.Math.EC.Multiplier
{
    public interface IPreCompCallback
    {
        PreCompInfo Precompute(PreCompInfo existing);
    }
}
