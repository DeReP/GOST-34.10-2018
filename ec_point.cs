using System;
using System.Numerics;

// класс описывающий точку эллиптичесикой кривой вида y^2 = x^3 + ax + b (mod p) и операции над ней
namespace digital_signature
{
    public class ec_point
    {
        BigInteger a;
        public BigInteger A
        {
            get
            {
                return a;
            }
        }

        BigInteger b;
        public BigInteger B
        {
            get
            {
                return b;
            }
        }

        BigInteger x;
        public BigInteger X
        {
            get
            {
                return x;
            }
        }

        BigInteger y;
        public BigInteger Y
        {
            get
            {
                return y;
            }
        }

        BigInteger p;
        public BigInteger P
        {
            get
            {
                return p;
            }
        }

        public ec_point(BigInteger a, BigInteger b, BigInteger p, BigInteger x, BigInteger y)
        {
            this.a = a;
            this.b = b;
            this.p = p;
            this.x = x;
            this.y = y;
        }

        public static ec_point operator +(ec_point point1, ec_point point2)
        {
            if (point1.A != point2.A || point1.B != point2.B || point1.P != point2.P)
                throw new Exception("скложение точек разных кривых");


            BigInteger lambda;

            if (point1 != point2)
            {
                BigInteger delta_y = (point2.Y - point1.Y) > 0 ? point2.Y - point1.Y : point2.Y - point1.Y + point1.P;
                BigInteger delta_x = (point2.X - point1.X) > 0 ? point2.X - point1.X : point2.X - point1.X + point1.P;

                if (delta_x == 0)
                    throw new Exception("при сложении точек получили точку О");

                lambda = delta_y * math.Ext_Euclidian(delta_x, point1.P);
            }
            else
            {
                lambda = (3 * (point1.X * point1.X) + point1.A) * math.Ext_Euclidian(2 * point1.Y, point1.p);
            }

            BigInteger X = math.mod(lambda * lambda - point1.X - point2.X, point1.P);
            BigInteger Y = math.mod(lambda * (point1.X - X) - point1.Y, point1.P);

            return new ec_point(point1.A, point1.B, point1.P, X, Y);

        }

        public static ec_point operator * (ec_point point, BigInteger k)
        {

            ec_point result = point;
            k -= 1;
            while(k > 0)
            {
                if (k % 2 != 0)
                {
                    result = result + point;
                    --k;
                }
                else
                {
                    k = k / 2;
                    point = point + point;
                }
                
            }

            return result;
        }

        public static ec_point operator * (BigInteger k, ec_point point)
        {
            return point * k;
        }
  
        public static bool operator == (ec_point point1, ec_point point2)
        {
            if (point1.A != point2.A || point1.B != point2.B || point1.P != point2.P)
                throw new Exception("сравнение точек разных кривых");

            if ((point1.X == point2.X) && (point1.Y == point2.Y))
                return true;

            return false;
        }

        public static bool operator != (ec_point point1, ec_point point2)
         {
            if (point1.A != point2.A || point1.B != point2.B || point1.P != point2.P)
                throw new Exception("сравнение точек разных кривых");

            if ((point1.X != point2.X) || (point1.Y  != point2.Y))
                return true;

            return false;
        }

    }
}
