using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exception = System.Exception;
using System.Numerics;

namespace HUL
{
    /* All (double) are placed for the absence of < for Fraction */
    public class Vector
    {
        public class V2D
        {
            public virtual Fraction X { get; set; }
            public virtual Fraction Y { get; set; }

            public V2D(Fraction x, Fraction y) { X = x; Y = y; }

            public virtual Fraction[] Vector
            {
                get => new Fraction[] { X, Y };
                set { if (value.Length == 2) { X = value[0]; Y = value[1]; } }
            }

            public virtual Fraction Magnitude => Math.Sqrt((X * X) + (Y * Y));

            public static Fraction Angle(V2D v1, V2D v2)
            {
                v1.Normalize();
                v2.Normalize();
                return Math.Acos(V2D.DotProduct(v1, v2));
            }

            public static Fraction Distance(V2D v1, V2D v2)
            {
                return Math.Sqrt(((v1.X - v2.X) * (v1.X - v2.X)) + ((v1.Y - v2.Y) * (v1.Y - v2.Y)));
            }

            public static Fraction DotProduct(V2D v1, V2D v2) { return ((v1.X * v2.X) + (v1.Y * v2.Y)); }

            public static V2D Interpolate(V2D v1, V2D v2, Fraction control)
            {
                V2D tempVecV2D = new V2D(0.0, 0.0)
                {
                    X = (v1.X * (1 - control)) + (v2.X * control),
                    Y = (v1.Y * (1 - control)) + (v2.Y * control)
                };
                return tempVecV2D;
            }

            public static V2D operator -(V2D v1, V2D v2) { return new V2D(v1.X - v2.X, v1.Y - v2.Y); }

            public static V2D operator -(V2D v1) { return new V2D(-v1.X, -v1.Y); }

            public static V2D operator *(V2D v1, Fraction d) { return new V2D(v1.X * d, v1.Y * d); }

            public static V2D operator *(Fraction d, V2D v1) { return new V2D(v1.X * d, v1.Y * d); }

            public static V2D operator /(V2D v1, Fraction d) { return new V2D(v1.X / d, v1.Y / d); }

            public static V2D operator +(V2D v1, V2D v2) { return new V2D(v1.X + v2.X, v1.Y + v2.Y); }

            public static bool operator <(V2D v1, V2D v2) { return (double)v1.Magnitude < v2.Magnitude; }

            public static bool operator <=(V2D v1, V2D v2) { return (double)v1.Magnitude <= v2.Magnitude; }

            public static bool operator !=(V2D v1, V2D v2) { return !(v1 == v2); }

            public static bool operator ==(V2D v1, V2D v2) { return (v1.X == v2.X && v1.Y == v2.Y); }

            public static bool operator >(V2D v1, V2D v2) { return (double)v1.Magnitude > v2.Magnitude; }

            public static bool operator >=(V2D v1, V2D v2) { return (double)v1.Magnitude >= v2.Magnitude; }

            public override int GetHashCode() { return (int)(X + Y) % Int32.MaxValue; }

            public override string ToString() { return "(" + X + "," + Y + ")"; }

            public override bool Equals(object obj)
            {
                V2D tempobj = obj as V2D;
                return (object)tempobj != null && this == tempobj;
            }

            public virtual void Normalize()
            {
                Fraction normal = Magnitude;
                if ((double)normal > 0)
                {
                    normal = 1 / normal;
                    X *= normal;
                    Y *= normal;
                }
            }

            public static explicit operator Matrix(V2D vector)
            {
                Matrix returnMatrix = new Matrix();
                Fraction[,] insertarray = new Fraction[2,1];
                insertarray[0, 0] = vector.Vector[0];
                insertarray[1, 0] = vector.Vector[1];
                returnMatrix.AddValues(insertarray);
                return returnMatrix;
            }

        }

        public class V3D
        {
            public virtual Fraction X { get; set; }
            public virtual Fraction Y { get; set; }
            public virtual Fraction Z { get; set; }

            public V3D(Fraction x, Fraction y, Fraction z) { X = x; Y = y; Z = z; }

            public virtual Fraction[] Vector
            {
                get => new Fraction[] { X, Y, Z };
                set { if (value.Length == 3) { X = value[0]; Y = value[1]; Z = value[2]; } }
            }

            public virtual Fraction Magnitude => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

            public static Fraction Angle(V3D v1, V3D v2)
            {
                v1.Normalize();
                v2.Normalize();
                return Math.Acos(V3D.DotProduct(v1, v2));
            }

            public static Fraction Distance(V3D v1, V3D v2)
            {
                return Math.Sqrt(((v1.X - v2.X) * (v1.X - v2.X)) + ((v1.Y - v2.Y) * (v1.Y - v2.Y)) + ((v1.Z - v2.Z) * (v1.Z - v2.Z)));
            }

            public static Fraction DotProduct(V3D v1, V3D v2) { return ((v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z)); }

            public static V3D Interpolate(V3D v1, V3D v2, Fraction control)
            {
                V3D tempVecV2D = new V3D(0.0, 0.0, 0.0)
                {
                    X = (v1.X * (1 - control)) + (v2.X * control),
                    Y = (v1.Y * (1 - control)) + (v2.Y * control),
                    Z = (v1.Z * (1 - control)) + (v2.Z * control)
                };
                return tempVecV2D;
            }

            public static V3D operator -(V3D v1, V3D v2) { return new V3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z); }

            public static V3D operator -(V3D v1) { return new V3D(-v1.X, -v1.Y, -v1.Z); }

            public static V3D operator *(V3D v1, Fraction d) { return new V3D(v1.X * d, v1.Y * d, v1.Z * d); }

            public static V3D operator *(Fraction d, V3D v1) { return new V3D(v1.X * d, v1.Y * d, v1.Z * d); }

            public static V3D operator *(V3D v1, V3D v2)
            {
                var tempVector = new V3D(0.0, 0.0, 0.0)
                {
                    X = (v1.Y * v2.Z) - (v1.Z * v2.Y),
                    Y = (v1.Z * v2.X) - (v1.X * v2.Z),
                    Z = (v1.X * v2.Y) - (v1.Y * v2.X)
                };
                return tempVector;
            }

            public static V3D operator /(V3D v1, Fraction d) { return new V3D(v1.X / d, v1.Y / d, v1.Z / d); }

            public static V3D operator +(V3D v1, V3D v2) { return new V3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z); }

            public static bool operator <(V3D v1, V3D v2) { return (double)v1.Magnitude < v2.Magnitude; }

            public static bool operator <=(V3D v1, V3D v2) { return (double)v1.Magnitude <= v2.Magnitude; }

            public static bool operator !=(V3D v1, V3D v2) { return !(v1 == v2); }

            public static bool operator ==(V3D v1, V3D v2) { return (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z); }

            public static bool operator >(V3D v1, V3D v2) { return (double)v1.Magnitude > v2.Magnitude; }

            public static bool operator >=(V3D v1, V3D v2) { return (double)v1.Magnitude >= v2.Magnitude; }

            public override int GetHashCode() { return (int)(X + Y + Z) % Int32.MaxValue; }

            public override string ToString() { return "(" + X + "," + Y + "," + Z + ")"; }

            public override bool Equals(object obj)
            {
                V3D tempobj = obj as V3D;
                return (object)tempobj != null && this == tempobj;
            }

            public virtual void Normalize()
            {
                Fraction normal = Magnitude;
                if ((double)normal > 0)
                {
                    normal = 1 / normal;
                    X *= normal;
                    Y *= normal;
                    Z *= normal;
                }
            }

            public static explicit operator Matrix(V3D vector)
            {
                Matrix returnMatrix = new Matrix();
                Fraction[,] insertarray = new Fraction[3, 1];
                insertarray[0, 0] = vector.Vector[0];
                insertarray[1, 0] = vector.Vector[1];
                insertarray[2, 0] = vector.Vector[2];
                returnMatrix.AddValues(insertarray);
                return returnMatrix;
            }

        }

        public class V4D
        {
            public virtual Fraction X { get; set; }
            public virtual Fraction Y { get; set; }
            public virtual Fraction Z { get; set; }
            public virtual Fraction W { get; set; }

            public V4D(Fraction x, Fraction y, Fraction z, Fraction w) { X = x; Y = y; Z = z; W = w; }

            public virtual Fraction[] Vector
            {
                get => new Fraction[] { X, Y, Z, W };
                set { if (value.Length == 4) { X = value[0]; Y = value[1]; Z = value[2]; W = value[3]; } }
            }

            public virtual Fraction Magnitude => Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));

            public static Fraction Angle(V4D v1, V4D v2)
            {
                v1.Normalize();
                v2.Normalize();
                return Math.Acos(V4D.DotProduct(v1, v2));
            }

            public static Fraction Distance(V4D v1, V4D v2)
            {
                return Math.Sqrt(((v1.X - v2.X) * (v1.X - v2.X)) + ((v1.Y - v2.Y) * (v1.Y - v2.Y)) + ((v1.Z - v2.Z) * (v1.Z - v2.Z)) + ((v1.W - v2.W) * (v1.W - v2.W)));
            }

            public static Fraction DotProduct(V4D v1, V4D v2) { return ((v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z) + (v1.W * v2.W)); }

            public static V4D Interpolate(V4D v1, V4D v2, Fraction control)
            {
                V4D tempVecV4D = new V4D(0.0, 0.0, 0.0, 0.0)
                {
                    X = (v1.X * (1 - control)) + (v2.X * control),
                    Y = (v1.Y * (1 - control)) + (v2.Y * control),
                    Z = (v1.Z * (1 - control)) + (v2.Z * control),
                    W = (v1.W * (1 - control)) + (v2.W * control)
                };
                return tempVecV4D;
            }

            public static V4D Cross4(V4D v1, V4D v2, V4D v3)
            {
                //Intermediate values
                //Fraction A = (v2.X * v3.Y) - (v2.Y * v3.X); 
                //Fraction B = (v2.X * v3.Z) - (v2.Z * v3.X); 
                //Fraction C = (v2.X * v3.W) - (v2.W * v3.X); 
                //Fraction D = (v2.Y * v3.Z) - (v2.Z * v3.Y); 
                //Fraction E = (v2.Y * v3.W) - (v2.W * v3.Y); 
                //Fraction F = (v2.Z * v3.W) - (v2.W * v1.Z); 

                V4D returnV4D = new V4D(0.0, 0.0, 0.0, 0.0)
                {
                    X = (v1.Y * ((v2.Z * v3.W) - (v2.W * v1.Z))) - (v1.Z * ((v2.Y * v3.W) - (v2.W * v3.Y))) +
                        (v1.W * ((v2.Y * v3.Z) - (v2.Z * v3.Y))),
                    Y = -(v1.X * ((v2.Z * v3.W) - (v2.W * v1.Z))) + (v1.Z * ((v2.X * v3.W) - (v2.W * v3.X))) -
                        (v1.W * ((v2.X * v3.Z) - (v2.Z * v3.X))),
                    Z = (v1.X * ((v2.Y * v3.W) - (v2.W * v3.Y))) - (v1.Y * ((v2.X * v3.W) - (v2.W * v3.X))) +
                        (v1.W * ((v2.X * v3.Y) - (v2.Y * v3.X))),
                    W = -(v1.X * ((v2.Y * v3.Z) - (v2.Z * v3.Y))) + (v1.Y * ((v2.X * v3.Z) - (v2.Z * v3.X))) -
                        (v1.Z * ((v2.X * v3.Y) - (v2.Y * v3.X)))
                };


                return returnV4D;
            }

            public static Vector.V4D Rotate4(Vector.V4D vector, double theta, string planeofrotation)
            {
                Matrix rotMatrix = new Matrix(4, 4);
                Matrix matrixvectorcopy = (Matrix) vector;

                switch (planeofrotation)
                {
                    case "XY":
                        rotMatrix.AddValues(new Fraction[,] { { Math.Cos(theta), Math.Sin(theta), 0, 0 }, { -Math.Sin(theta), Math.Cos(theta), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } });
                        return (Vector.V4D)(rotMatrix * matrixvectorcopy);
                    case "YZ":
                        rotMatrix.AddValues(new Fraction[,] { { 1, 0, 0, 0 }, { 0, Math.Cos(theta), Math.Sin(theta), 0 }, { 0, -Math.Sin(theta), Math.Cos(theta), 0 }, { 0, 0, 0, 1 } });
                        return (Vector.V4D)(rotMatrix * matrixvectorcopy);
                    case "ZX":
                        rotMatrix.AddValues(new Fraction[,] { { Math.Cos(theta), 0, -Math.Sin(theta), 0 }, { 0, 1, 0, 0 }, { Math.Sin(theta), 0, Math.Cos(theta), 0 }, { 0, 0, 0, 1 } });
                        return (Vector.V4D)(rotMatrix * matrixvectorcopy);
                    case "XW":
                        rotMatrix.AddValues(new Fraction[,] { { Math.Cos(theta), 0, 0, Math.Sin(theta) }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { -Math.Sin(theta), 0, 0, Math.Cos(theta) } });
                        return (Vector.V4D)(rotMatrix * matrixvectorcopy);
                    case "YW":
                        rotMatrix.AddValues(new Fraction[,] { { 1, 0, 0, 0 }, { 0, Math.Cos(theta), 0, -Math.Sin(theta) }, { 0, 0, 1, 0 }, { 0, Math.Sin(theta), 0, Math.Cos(theta) } });
                        return (Vector.V4D)(rotMatrix * matrixvectorcopy);
                    case "ZW":
                        rotMatrix.AddValues(new Fraction[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, Math.Cos(theta), -Math.Sin(theta) }, { 0, 0, Math.Sin(theta), Math.Cos(theta) } });
                        return (Vector.V4D)(rotMatrix * matrixvectorcopy);
                }

                return new Vector.V4D(0, 0, 0, 0);
            }

            public static V4D operator -(V4D v1, V4D v2) { return new V4D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W); }

            public static V4D operator -(V4D v1) { return new V4D(-v1.X, -v1.Y, -v1.Z, -v1.W); }

            public static V4D operator *(V4D v1, Fraction d) { return new V4D(v1.X * d, v1.Y * d, v1.Z * d, v1.W * d); }

            public static V4D operator *(Fraction d, V4D v1) { return new V4D(v1.X * d, v1.Y * d, v1.Z * d, v1.W * d); }

            // 4D Cross product needs new implementation
            //public static V4D operator *(V4D v1, V4D v2)
            //{
            //    var tempVector = new V4D(0.0, 0.0, 0.0, 0.0)
            //    {
            //        X = (v1.Y * v2.Z) - (v1.Z * v2.Y),
            //        Y = (v1.Z * v2.X) - (v1.X * v2.Z),
            //        Z = (v1.X * v2.Y) - (v1.Y * v2.X),
            //        W = (v1.X * v2.Y) - (v1.Y * v2.X)
            //    };
            //    return tempVector;
            //}

            public static V4D operator /(V4D v1, Fraction d) { return new V4D(v1.X / d, v1.Y / d, v1.Z / d, v1.W / d); }

            public static V4D operator +(V4D v1, V4D v2) { return new V4D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W); }

            public static bool operator <(V4D v1, V4D v2) { return (double)v1.Magnitude < v2.Magnitude; }

            public static bool operator <=(V4D v1, V4D v2) { return (double)v1.Magnitude <= v2.Magnitude; }

            public static bool operator !=(V4D v1, V4D v2) { return !(v1 == v2); }

            public static bool operator ==(V4D v1, V4D v2) { return (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W); }

            public static bool operator >(V4D v1, V4D v2) { return (double)v1.Magnitude > v2.Magnitude; }

            public static bool operator >=(V4D v1, V4D v2) { return (double)v1.Magnitude >= v2.Magnitude; }

            public override int GetHashCode() { return (int)(X + Y + Z + W) % Int32.MaxValue; }

            public override string ToString() { return "(" + X + "," + Y + "," + Z + "," + W + ")"; }

            public override bool Equals(object obj)
            {
                V4D tempobj = obj as V4D;
                return (object)tempobj != null && this == tempobj;
            }

            public virtual void Normalize()
            {
                Fraction normal = Magnitude;
                if ((double)normal > 0)
                {
                    normal = 1 / normal;
                    X *= normal;
                    Y *= normal;
                    Z *= normal;
                    W *= normal;
                }
            }

            public static explicit operator Matrix(V4D vector)
            {
                Matrix returnMatrix = new Matrix();
                Fraction[,] insertarray = new Fraction[4, 1];
                insertarray[0, 0] = vector.Vector[0];
                insertarray[1, 0] = vector.Vector[1];
                insertarray[2, 0] = vector.Vector[2];
                insertarray[3, 0] = vector.Vector[3];
                returnMatrix.AddValues(insertarray);
                return returnMatrix;
            }

        }

        public class VND
        {
            public virtual int Dimension { get; set; }

            private Fraction[] _vectorVariables;
            public virtual Fraction[] Vector
            {
                get => _vectorVariables;
                set { if (value.Length == Dimension) { _vectorVariables = value; } }
            }

            public VND(int dimension)
            {
                Dimension = dimension;
                _vectorVariables = new Fraction[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    Vector[i] = 0;
                }
            }

            public VND(Fraction[] vector)
            {
                Vector = vector;
                Dimension = vector.Length;
            }

            public virtual Fraction Magnitude
            {
                get
                {
                    Fraction squaresum = 0.0;
                    for (int i = 0; i < Dimension; i++)
                    {
                        squaresum += Math.Pow(_vectorVariables[i], 2);
                    }

                    return Math.Sqrt(squaresum);
                }
            }

            public static Fraction Angle(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    v1.Normalize();
                    v2.Normalize();
                    return Math.Acos(VND.DotProduct(v1, v2));
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static Fraction Distance(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Fraction squaresum = 0.0;
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        squaresum += Math.Pow(v1.Vector[i] - v2.Vector[i], 2);
                    }

                    return squaresum;
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static Fraction DotProduct(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Fraction sum = 0.0;
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        sum += v1.Vector[i] * v2.Vector[i];
                    }

                    return sum;
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static VND operator -(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Fraction[] returnVectorFractionArray = new Fraction[v1.Dimension];
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        returnVectorFractionArray[i] = v1.Vector[i] - v2.Vector[i];
                    }

                    return new VND(returnVectorFractionArray);
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static VND operator -(VND v1)
            {
                Fraction[] returnVectorFractionArray = new Fraction[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorFractionArray[i] = -v1.Vector[i];
                }

                return new VND(returnVectorFractionArray);
            }

            public static VND operator *(VND v1, Fraction d)
            {
                Fraction[] returnVectorFractionArray = new Fraction[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorFractionArray[i] = v1.Vector[i] * d;
                }

                return new VND(returnVectorFractionArray);
            }

            public static VND operator *(Fraction d, VND v1)
            {
                Fraction[] returnVectorFractionArray = new Fraction[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorFractionArray[i] = v1.Vector[i] * d;
                }

                return new VND(returnVectorFractionArray);
            }

            public static VND operator /(VND v1, Fraction d)
            {
                Fraction[] returnVectorFractionArray = new Fraction[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorFractionArray[i] = v1.Vector[i] / d;
                }

                return new VND(returnVectorFractionArray);
            }

            public static VND operator +(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Fraction[] returnVectorFractionArray = new Fraction[v1.Dimension];
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        returnVectorFractionArray[i] = v1.Vector[i] + v2.Vector[i];
                    }

                    return new VND(returnVectorFractionArray);
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static bool operator <(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                    return v1.Magnitude < v2.Magnitude;

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static bool operator <=(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                    return v1.Magnitude <= v2.Magnitude;

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static bool operator !=(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                    return !(v1 == v2);

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static bool operator ==(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        if (v1.Vector[i] != v2.Vector[i])
                            return false;
                    }
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static bool operator >(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                    return v1.Magnitude > v2.Magnitude;

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static bool operator >=(VND v1, VND v2)
            {
                if (v1.Dimension == v2.Dimension)
                    return v1.Magnitude >= v2.Magnitude;

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public override int GetHashCode()
            {
                int sum = 0;
                for (int i = 0; i < Dimension; i++)
                {
                    sum += _vectorVariables[i];
                }
                return sum % Int32.MaxValue;
            }

            public override string ToString()
            {
                StringBuilder newBuilder = new StringBuilder();
                newBuilder.Append("(");
                for (int i = 0; i < Dimension; i++)
                {
                    newBuilder.Append(_vectorVariables[i].ToString());
                    newBuilder.Append(",");
                }
                newBuilder.Append(")");
                return newBuilder.ToString();
            }

            public override bool Equals(object obj)
            {
                VND tempobj = obj as VND;
                return (object)tempobj != null && this == tempobj;
            }

            public virtual void Normalize()
            {
                Fraction normal = Magnitude;
                if ((double)normal > 0)
                {
                    normal = 1 / normal;

                    for (int i = 0; i < Dimension; i++)
                    {
                        _vectorVariables[i] *= normal;
                    }
                }
            }

            public static explicit operator Matrix(VND vector)
            {
                Matrix returnMatrix = new Matrix();
                Fraction[,] insertarray = new Fraction[vector.Dimension, 1];
                for (int i = 0; i < vector.Dimension; i++)
                {
                    insertarray[i, 0] = vector.Vector[i];
                }
                returnMatrix.AddValues(insertarray);
                return returnMatrix;
            }
        }

        public class CVND
        {
            public virtual int Dimension { get; set; }

            private Complex[] _vectorVariables;
            public virtual Complex[] Vector
            {
                get => _vectorVariables;
                set { if (value.Length == Dimension) { _vectorVariables = value; } }
            }

            public CVND(int dimension)
            {
                Dimension = dimension;
                _vectorVariables = new Complex[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    Vector[i] = 0;
                }
            }

            public CVND(Complex[] vector)
            {
                Dimension = vector.Length;
                Vector = vector;
            }

            public virtual Complex Magnitude
            {
                get
                {
                    Complex squaresum = 0.0;
                    for (int i = 0; i < Dimension; i++)
                    {
                        squaresum += _vectorVariables[i] * Complex.Conjugate(_vectorVariables[i]);
                    }

                    return Complex.Sqrt(squaresum);
                }
            }

            public static Complex Angle(CVND v1, CVND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    v1.Normalize();
                    v2.Normalize();
                    return Complex.Acos(CVND.DotProduct(v1, v2));
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static Complex Distance(CVND v1, CVND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Complex squaresum = 0.0;
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        squaresum += Complex.Pow(v1.Vector[i] - v2.Vector[i], 2);
                    }

                    return squaresum;
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static Complex DotProduct(CVND v1, CVND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Complex sum = 0.0;
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        sum += v1.Vector[i] * v2.Vector[i];
                    }

                    return sum;
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static CVND operator -(CVND v1, CVND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Complex[] returnVectorComplexArray = new Complex[v1.Dimension];
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        returnVectorComplexArray[i] = v1.Vector[i] - v2.Vector[i];
                    }

                    return new CVND(returnVectorComplexArray);
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static CVND operator -(CVND v1)
            {
                Complex[] returnVectorComplexArray = new Complex[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorComplexArray[i] = -v1.Vector[i];
                }

                return new CVND(returnVectorComplexArray);
            }

            public static CVND operator *(CVND v1, Complex d)
            {
                Complex[] returnVectorComplexArray = new Complex[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorComplexArray[i] = v1.Vector[i] * d;
                }

                return new CVND(returnVectorComplexArray);
            }

            public static CVND operator *(Complex d, CVND v1)
            {
                Complex[] returnVectorComplexArray = new Complex[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorComplexArray[i] = v1.Vector[i] * d;
                }

                return new CVND(returnVectorComplexArray);
            }

            public static CVND operator /(CVND v1, Complex d)
            {
                Complex[] returnVectorComplexArray = new Complex[v1.Dimension];
                for (int i = 0; i < v1.Dimension; i++)
                {
                    returnVectorComplexArray[i] = v1.Vector[i] / d;
                }

                return new CVND(returnVectorComplexArray);
            }

            public static CVND operator +(CVND v1, CVND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    Complex[] returnVectorComplexArray = new Complex[v1.Dimension];
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        returnVectorComplexArray[i] = v1.Vector[i] + v2.Vector[i];
                    }

                    return new CVND(returnVectorComplexArray);
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            //public static bool operator <(CVND v1, CVND v2)
            //{
            //    if (v1.Dimension == v2.Dimension)
            //        return v1.Magnitude < v2.Magnitude;

            //    throw new Exception("One or both of the vectors have non matching dimensions.");
            //}

            //public static bool operator <=(CVND v1, CVND v2)
            //{
            //    if (v1.Dimension == v2.Dimension)
            //        return v1.Magnitude <= v2.Magnitude;

            //    throw new Exception("One or both of the vectors have non matching dimensions.");
            //}

            public static bool operator !=(CVND v1, CVND v2)
            {
                if (v1.Dimension == v2.Dimension)
                    return !(v1 == v2);

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            public static bool operator ==(CVND v1, CVND v2)
            {
                if (v1.Dimension == v2.Dimension)
                {
                    for (int i = 0; i < v1.Dimension; i++)
                    {
                        if (v1.Vector[i] != v2.Vector[i])
                            return false;
                    }
                }

                throw new Exception("One or both of the vectors have non matching dimensions.");
            }

            //public static bool operator >(CVND v1, CVND v2)
            //{
            //    if (v1.Dimension == v2.Dimension)
            //        return v1.Magnitude > v2.Magnitude;

            //    throw new Exception("One or both of the vectors have non matching dimensions.");
            //}

            //public static bool operator >=(CVND v1, CVND v2)
            //{
            //    if (v1.Dimension == v2.Dimension)
            //        return v1.Magnitude >= v2.Magnitude;

            //    throw new Exception("One or both of the vectors have non matching dimensions.");
            //}

            //public override int GetHashCode()
            //{
            //    int sum = 0;
            //    for (int i = 0; i < Dimension; i++)
            //    {
            //        sum += _vectorVariables[i];
            //    }
            //    return sum % Int32.MaxValue;
            //}

            public override string ToString()
            {
                StringBuilder newBuilder = new StringBuilder();
                newBuilder.Append("(");
                for (int i = 0; i < Dimension; i++)
                {
                    newBuilder.Append(_vectorVariables[i].ToString());
                    newBuilder.Append(",");
                }
                newBuilder.Append(")");
                return newBuilder.ToString();
            }

            public override bool Equals(object obj)
            {
                CVND tempobj = obj as CVND;
                return (object)tempobj != null && this == tempobj;
            }

            public override int GetHashCode()
            {
                int sum = 0;
                for (int i = 0; i < Dimension; i++)
                {
                    sum += _vectorVariables[i].GetHashCode();
                }
                return sum % Int32.MaxValue;
            }

            public virtual void Normalize()
            {
                Complex normal = Magnitude;
                if (normal != 0)
                {
                    normal = 1 / normal;

                    for (int i = 0; i < Dimension; i++)
                    {
                        _vectorVariables[i] *= normal;
                    }
                }
            }

            //public static explicit operator Matrix(CVND vector)
            //{
            //    Matrix returnMatrix = new Matrix();
            //    Complex[,] insertarray = new Complex[vector.Dimension, 1];
            //    for (int i = 0; i < vector.Dimension; i++)
            //    {
            //        insertarray[i, 0] = vector.Vector[i];
            //    }
            //    returnMatrix.AddValues(insertarray);
            //    return returnMatrix;
            //}
        }

        public class VecVis
        {
            public enum ProjectionType
            {
                Parallel = 0,
                Perspective = 1
            }

            public static V3D[] CalTransMatrix3(V3D from, V3D to, V3D up)
            {
                //V3D nullvector = new V3D(0, 0, 0);
                V3D vectorC = to - from;
                try
                {
                    vectorC.Normalize();
                }
                catch
                {
                    throw new Exception("To vector & From vector are the same");
                }
                
                V3D vectorA = vectorC * up;
                try
                {
                    vectorA.Normalize();
                }
                catch
                {
                    throw new Exception("Up vector is parallel to the line of sight");
                }

                V3D vectorB = vectorA * vectorC;
                return new V3D[3] { vectorA, vectorB, vectorC };
            }

            public static void Project3To2(V3D from, V3D[] vavbvc, V3D[] vertarray, int numvert, double radius, double vangle, double cx, double cy, double lx, double ly, ProjectionType projtype)
            {
                double S = 0, T = 0;
                V3D V;

                if (projtype == ProjectionType.Parallel)
                {
                    S = 1 / radius;
                }
                else
                {
                    T = 1 / Math.Tan(vangle / 2);
                }

                for (int i = 0; i < numvert; i++)
                {
                    V = vertarray[i] - from;

                    if (projtype == ProjectionType.Perspective)
                    {
                        S = T / V3D.DotProduct(V, vavbvc[2]);
                    }

                    vertarray[i].X = cx + (lx * S * V3D.DotProduct(V, vavbvc[0]));
                    vertarray[i].Y = cy + (ly * S * V3D.DotProduct(V, vavbvc[1]));
                }
            }

            public static V4D[] CalTransMatrix4(V4D from, V4D to, V4D up, V4D over)
            {
                //V4D nullvector = new V4D(0, 0, 0, 0);
                V4D vectorD = to - from;
                try
                {
                    vectorD.Normalize();
                }
                catch
                {
                    throw new Exception("To vector & From vector are the same");
                }

                V4D vectorA = V4D.Cross4(up, over, vectorD);
                try
                {
                    vectorA.Normalize();
                }
                catch
                {
                    throw new Exception("Invalid Up vector");
                }

                V4D vectorB = V4D.Cross4(over, vectorD, vectorA);
                try
                {
                    vectorB.Normalize();
                }
                catch
                {
                    throw new Exception("Invalid Over vector");
                }

                V4D vectorC = V4D.Cross4(vectorD, vectorA, vectorB);
                return new V4D[4] { vectorA, vectorB, vectorC, vectorD };
            }

            public static void Project4To3(V4D from, V4D[] vavbvcvd, V4D[] vertarray, int numvert, double radius, double vangle, ProjectionType projtype)
            {
                double S = 0, T = 0;
                V4D V;

                if (projtype == ProjectionType.Parallel)
                {
                    S = 1 / radius;
                }
                else
                {
                    T = 1 / Math.Tan(vangle / 2);
                }

                for (int i = 0; i < numvert; i++)
                {
                    V = vertarray[i] - from;

                    if (projtype == ProjectionType.Perspective)
                    {
                        S = T / V4D.DotProduct(V, vavbvcvd[3]);
                    }

                    vertarray[i].X = S * V4D.DotProduct(V, vavbvcvd[0]);
                    vertarray[i].Y = S * V4D.DotProduct(V, vavbvcvd[1]);
                    vertarray[i].Z = S * V4D.DotProduct(V, vavbvcvd[2]);
                }
            }
        }
    }  
}
