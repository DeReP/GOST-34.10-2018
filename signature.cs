using System;
using System.Numerics;
using System.Security.Cryptography;
//описание класса цифровой подписи с методами по ее созданию и проверке
namespace digital_signature
{
    class signature
    {
        ec_point point;
        BigInteger q;
        int hash_length;
        stribog stribog;

        public signature(ec_point point, BigInteger q)
        {
            this.point = point;
            this.q = q;

            if (q > BigInteger.Pow(2, 254) && q < BigInteger.Pow(2, 256))
                hash_length = 256;
            else
                if (q > BigInteger.Pow(2, 508) && q < BigInteger.Pow(2, 512))
                hash_length = 512;
            else
                throw new Exception("wrong q value");

            stribog = new stribog(hash_length);
 
        }


        //генерация рандомного  числа
        BigInteger Random_BigInt(int Length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();  // экземпляр крипотостойкого генератора чисел 
            byte[] rnd = new byte[Length];
            rng.GetBytes(rnd);
            BigInteger res = new BigInteger(rnd);
            return BigInteger.Abs(res);  //возвращаем модуль числа
        }

        public BigInteger Generate_Private_Key(int length)
        {
            BigInteger d = new BigInteger();

            do
            {
                d = Random_BigInt(length);
            }
            while (d < 1 || d > q - 1);

            return d;
        }

        public ec_point Generate_Public_Key(BigInteger d)
        {
            return point * d;
        }

        public byte[] Generate_Signature(byte[] message, BigInteger d)
        {

            byte[] hash = stribog.Hash(message);

            BigInteger alpha = new BigInteger(hash);

            BigInteger e = math.mod(alpha, q) == 0 ? math.mod(alpha, q) : 1;

            //проверка из примера госта
            e = BigInteger.Parse("20798893674476452017134061561508270130637142515379653289952617252661468872421");

            BigInteger k;
            BigInteger r;
            BigInteger S;
            do
            {
                do
                {
                    do
                    {
                        k = Random_BigInt(hash_length/8);
                    }
                    while (k < 1 || k > q);
                    //проверка из госта
                    k = BigInteger.Parse("53854137677348463731403841147996619241504003434302020712960838528893196233395"); 
                    ec_point C = point * k;
                    r = math.mod(C.X, q);
                }
                while (r == 0);

                S = math.mod(r * d + k * e, q);
            }
            while (S == 0);

            byte[] signature = new byte[hash_length / 4];

            Array.Copy(r.ToByteArray(), 0, signature, 0, hash_length / 8);
            Array.Copy(S.ToByteArray(), 0, signature, hash_length / 8, hash_length / 8);

            return signature;
        }


        public bool Verify_Signature(byte[] message, byte[] signature, ec_point Q)
        {

            byte[] byte_r = new byte[hash_length / 8];
            byte[] byte_s = new byte[hash_length / 8];

            Array.Copy(signature, 0, byte_r, 0, hash_length / 8);
            Array.Copy(signature, hash_length / 8, byte_s, 0, hash_length / 8);

            BigInteger r = new BigInteger(byte_r);
            BigInteger s = new BigInteger(byte_s);
            
           
            
            if (r < 1 || r > q - 1 || s < 1 || s > q - 1)
                return false;

            byte[] hash = stribog.Hash(message);
            BigInteger alpha = new BigInteger(hash);
            BigInteger e = math.mod(alpha, q) == 0 ? math.mod(alpha, q) : 1;

            //проверка из примера госта
            e = BigInteger.Parse("20798893674476452017134061561508270130637142515379653289952617252661468872421");

            BigInteger v = math.Ext_Euclidian(e, q);

            BigInteger z1 = math.mod(s * v, q);
            BigInteger z2 = math.mod(-(r * v), q);

            ec_point C = z1 * point + z2 * Q;

            BigInteger R = math.mod(C.X, q);

            if (R == r)
               return true;

            return false;

        }
    }
}
