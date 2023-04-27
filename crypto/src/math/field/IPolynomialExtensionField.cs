using System;

namespace Linkhub.BouncyCastle.Math.Field
{
    public interface IPolynomialExtensionField
        : IExtensionField
    {
        IPolynomial MinimalPolynomial { get; }
    }
}
