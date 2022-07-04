using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HUL
{
    public class Matrix
    {
        public int Row { get; internal set; }
        public int Column { get; internal set; }
        public Fraction[,] Values { get; set; }

        public Matrix(int row, int column)
        {
            Row = row;
            Column = column;
            Values = InitializeArray(row, column);
        }

        public Matrix() { }

        public void AddValues(Fraction[,] values)
        {
            Row = values.GetLength(0);
            Column = values.GetLength(1);
            Values = values;
        }

        public Fraction this[int x, int y]
        {
            get { return Values[x, y]; }
            set { Values[x, y] = value; }
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix tempMatrix = new Matrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Row; j++) { tempMatrix[i, j] = a[i, j] - b[i, j]; }
            }

            return tempMatrix;
        }

        public static Matrix operator -(Matrix a)
        {
            Matrix tempMatrix = new Matrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = -a[i, j]; }
            }

            return tempMatrix;
        }

        public static bool operator !=(Matrix a, Matrix b) { return !(a == b); }

        public static bool operator ==(Matrix a, Matrix b)
        { 
            if ((object)a == null && (object)b == null) { return true; }
            if ((object)a == null) { return false; }
            if ((object)b == null) { return false; }
            if (a.Row != b.Row || a.Column != b.Column) { return false; }

            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++)
                {
                    if (a[i, j] != b[i, j]) { return false; }
                }
            }

            return true;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Column == b.Row)
            {
                var tempMatrix = new Matrix(a.Row, b.Column);
                //tempMatrix.AddValues(new Fraction[a.Row, b.Column]);
                for (int i = 0; i < a.Row; i++)
                {
                    for (int j = 0; j < b.Column; j++)
                    {
                        for (int r = 0; r < b.Row; r++) { tempMatrix[i, j] += (a[i, r] * b[r, j]); }
                    }
                }

                return tempMatrix;
            }
            else { throw new ArgumentException("Row/Column of the matrices do not seem to match!"); }
        }

        public static Matrix operator *(Matrix a, Fraction constant)
        {
            Matrix tempMatrix = new Matrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = constant * a[i, j]; }
            }

            return tempMatrix;
        }

        public static Matrix operator *(Fraction constant, Matrix a)
        {
            Matrix tempMatrix = new Matrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = constant * a[i, j]; }
            }

            return tempMatrix;
        }

        public static Matrix operator /(Matrix a, Fraction constant)
        {
            return a * (1 / constant);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix tempMatrix = new Matrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = a[i, j] + b[i, j]; }
            }

            return tempMatrix;
        }

        public static void CopyTo(Matrix a, Matrix b)
        {
            for (int i = 0; i < b.Row; i++)
            {
                for (int j = 0; j < b.Column; j++) { b[i, j] = a[i, j]; }
            }
        }

        public Matrix[] LUDecomposition()
        {
            int n = Row;
            Matrix temp = new Matrix(Row, Column);
            //temp.AddValues(new Fraction[Row, Column]);
            CopyTo(this, temp);
            Matrix L = new Matrix(n, n);
            Matrix U = new Matrix(n, n);
            //L.AddValues(new Fraction[n, n]);
            //U.AddValues(new Fraction[n, n]);
            for (int j = 0; j < Column; j++) { U[0, j] = temp[0, j]; }
            //for (int i = 0; i < n; i++) { U[i, i] = this[i, i]; }
            //for (int j = 1; j < Column; j++) { for (int i = 0; i < Row - 1; i++) { U[i, j] = this[i, j]; } }
            //for (int i = 0; i < n; i++) { U[i, i] = 0; }
            //for (int j = 1; j < Column; j++) { for (int i = 0; i < Row - 1; i++) { L[i, j] = 0; } }
            for (int i = 0; i < n; i++) { L[i, i] = 1; }

            for (int k = 0; k < n; k++)
            {
                U[k, k] = temp[k, k];
                for (int i = k + 1; i < n; i++)
                {
                    L[i, k] = (temp[i, k] / U[k, k]);
                    U[k, i] = temp[k, i];
                }

                for (int i = k + 1; i < n; i++)
                {
                    for (int j = k + 1; j < n; j++) { temp[i, j] = temp[i, j] - (L[i, k] * U[k, j]); }
                }
            }

            return new Matrix[2] { L, U };
        }

        public Matrix[] LUPDecomposition()
        {
            int n = Row;
            int[] piearray = new int[n];
            Matrix temp = new Matrix(n, n);
            //temp.AddValues(new Fraction[n, n]);
            CopyTo(this, temp);
            Matrix P = new Matrix(n, n);
            //P.AddValues(new Fraction[n, n]);
            int kprime = 0;

            for (int i = 0; i < n; i++) { piearray[i] = i; }

            for (int k = 0; k < n; k++)
            {
                Fraction p = 0;
                for (int i = k; i < n; i++)
                {
                    if (Math.Abs((double)temp[i, k]) > p) { p = Math.Abs((double)temp[i, k]); kprime = i; }
                }
                if (p == (Fraction)0) { throw new Exception("Singular Matrix!"); }
                int tempvalueholder = piearray[kprime]; /* changed k to kprime */
                piearray[kprime] = piearray[k];
                piearray[k] = tempvalueholder;
                for (int i = 0; i < n; i++)
                {
                    Fraction tempvalueholder2 = temp[kprime, i]; /* changed k to kprime */
                    temp[kprime, i] = temp[k, i];
                    temp[k, i] = tempvalueholder2;
                }

                for (int i = k + 1; i < n; i++)
                {
                    temp[i, k] = (temp[i, k] / temp[k, k]);
                    for (int j = k + 1; j < n; j++) { temp[i, j] = temp[i, j] - (temp[i, k] * temp[k, j]); }
                }
            }

            int rcounter = 0;
            foreach (int cnumber in piearray) { P[rcounter, cnumber] = 1; rcounter++; }

            return new Matrix[2] {P, temp};
        }

        public Fraction NDeterminant() /* Fix Bug */
        {
            if (Row == Column)
            {
                if (Row == 2) { return (this[0, 0] * this[1, 1]) - (this[0, 1] * this[1, 0]); }
                Fraction result = 0.0;

                for (int i = 0; i < Row; i++)
                {
                    Matrix tempMatrix = new Matrix(Row - 1, Column - 1);
                    //tempMatrix.AddValues(new Fraction[Row - 1, Column - 1]);
                    int RowCount = 0;

                    for (int j = 0; j < Row; j++)
                    {
                        if (j != i)
                        {
                            for (int k = 1; k < Column; k++) { tempMatrix[RowCount, k - 1] = this[j, k]; }
                            RowCount++;
                        }

                    }

                    if (i % 2 == 0) { result += tempMatrix.NDeterminant(); }
                    else { result += tempMatrix.NDeterminant(); }
                }

                return result;
            }
            else { return 0; }
        }

        public Fraction GDeterminant()
        {
            if (Row == Column)
            {
                Matrix tempMatrix = LUDecomposition()[1];
                Fraction result = 1.0;
                for (int i = 0; i < tempMatrix.Row; i++) { result *= tempMatrix[i, i]; }

                return result;
            }
            else { return 0; }
        }

        public int Nullity()
        {
            Matrix temp = RREF();
            int counter = 0;
            for (int i = 0; i < Row; i++) { if (temp[i, i] == (Fraction)1) { counter++; } }

            return Row - counter;
        }

        public static Matrix Identity(int degree)
        {
            Matrix newMatrix = new Matrix(degree, degree);
            //newMatrix.AddValues(new Fraction[row,column]);
            for (int i = 0; i < degree; i++) { newMatrix[i, i] = 1.0; }

            return newMatrix;
        }

        public Matrix RREF()
        {
            Matrix tempMatrix = new Matrix(Row, Column);
            //tempMatrix.AddValues(new Fraction[Row, Column]);
            CopyTo(this, tempMatrix);
            int lead = 0, rowCount = Row, columnCount = Column;
            for (int r = 0; r < rowCount; r++)
            {
                if (columnCount <= lead) break;
                int i = r;
                while (tempMatrix[i, lead] == (Fraction)0)
                {
                    i++;
                    if (i == rowCount)
                    {
                        i = r;
                        lead++;
                        if (columnCount == lead)
                        {
                            lead--;
                            break;
                        }
                    }
                }
                for (int j = 0; j < columnCount; j++)
                {
                    Fraction temp = tempMatrix[r, j];
                    tempMatrix[r, j] = tempMatrix[i, j];
                    tempMatrix[i, j] = temp;
                }
                Fraction div = tempMatrix[r, lead];
                if (div != (Fraction)0)
                    for (int j = 0; j < columnCount; j++) tempMatrix[r, j] /= div;
                for (int j = 0; j < rowCount; j++)
                {
                    if (j != r)
                    {
                        Fraction sub = tempMatrix[j, lead];
                        for (int k = 0; k < columnCount; k++) tempMatrix[j, k] -= (sub * tempMatrix[r, k]);
                    }
                }
                lead++;
            }

            return tempMatrix;
        }

        public Matrix RetrieveColumn(int column)
        {
            Matrix newMatrix = new Matrix(Row, 1);
            Matrix thisholder = new Matrix(Row, Column);
            //thisholder.AddValues(new Fraction[Row, Column]);
            CopyTo(this, thisholder);
            //newMatrix.AddValues(new Fraction[Column, 1]);
            Fraction[,] columnarray = new Fraction[Row, 1];

            for (int i = 0; i < Row; i++) { columnarray[i, 0] = thisholder[i, column]; }
            newMatrix.AddValues(columnarray);

            return newMatrix;
        }

        public Matrix RetrieveRow(int row)
        {
            Matrix newMatrix = new Matrix(1, Column);
            Matrix thisholder = new Matrix(Row, Column);
            //thisholder.AddValues(new Fraction[Row, Column]);
            CopyTo(this, thisholder);
            //newMatrix.AddValues(new Fraction[1, Column]);
            Fraction[,] rowarray = new Fraction[1, Column];

            for (int i = 0; i < Column; i++) { rowarray[0, i] = thisholder[row, i]; }
            newMatrix.AddValues(rowarray);

            return newMatrix;
        }

        public bool IsSingular() { if (GDeterminant() == (Fraction)0) { return true; } else { return false; } }

        public DLList<DLList<Matrix>> RSpace()
        {
            Matrix temp = this.RREF();
            var returnlist = new DLList<DLList<Matrix>>();
            returnlist.Append(new DLList<Matrix>());
            returnlist.Append(new DLList<Matrix>());

            for (int i = 0; i < Row; i++)
            {
                if (temp[i, i] == (Fraction)1)
                {
                    //double[,] temparray = new double[1, Column];
                    //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
                    Matrix insertionMatrix = temp.RetrieveRow(i);
                    Matrix tempMatrix = new Matrix(1, Row);
                    //tempMatrix.AddValues(new Fraction[1, Row]);
                    CopyTo(insertionMatrix, tempMatrix);
                    ((DLList<Matrix>)returnlist[0]).Append(tempMatrix);
                }
                else if (temp[i, i] == (Fraction)0)
                {
                    //double[,] temparray = new double[1, Column];
                    //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
                    Matrix insertionMatrix = temp.RetrieveRow(i);
                    Matrix tempMatrix = new Matrix(1, Row);
                    //tempMatrix.AddValues(new Fraction[1, Row]);
                    CopyTo(insertionMatrix, tempMatrix);
                    ((DLList<Matrix>)returnlist[1]).Append(insertionMatrix);
                }
            }

            return returnlist;
        }

        public DLList<DLList<Matrix>> CSpace()
        {
            Matrix temp = RREF();
            var returnlist = new DLList<DLList<Matrix>>();
            returnlist.Append(new DLList<Matrix>());
            returnlist.Append(new DLList<Matrix>());

            for (int i = 0; i < Row; i++)
            {
                if (temp[i, i] == (Fraction)1)
                {
                    //double[,] temparray = new double[1, Column];
                    //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
                    Matrix insertionMatrix = RetrieveColumn(i);
                    Matrix tempMatrix = new Matrix(Row, 1);
                    //tempMatrix.AddValues(new Fraction[Column, 1]);
                    CopyTo(insertionMatrix, tempMatrix);
                    ((DLList<Matrix>)returnlist[0]).Append(tempMatrix);
                }
                else if (temp[i, i] == (Fraction)0)
                {
                    //double[,] temparray = new double[1, Column];
                    //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
                    Matrix insertionMatrix = RetrieveColumn(i);
                    Matrix tempMatrix = new Matrix(Column, 1);
                    //tempMatrix.AddValues(new Fraction[Column, 1]);
                    CopyTo(insertionMatrix, tempMatrix);
                    ((DLList<Matrix>)returnlist[1]).Append(insertionMatrix);
                }
            }

            return returnlist;
        }

        public Matrix Transpose()
        {
            var tempValues = new Matrix(Row, Column);
            //tempValues.AddValues(new Fraction[Row, Column]);

            for (int x = 0; x < Row; ++x)
            {
                for (int y = 0; y < Column; ++y) { tempValues[y, x] = Values[x, y]; }
            }
            
            return tempValues;
        }

        public override bool Equals(object obj)
        {
            var tempobj = obj as Matrix;
            return (tempobj != null) && (this == tempobj);
        }

        public override int GetHashCode()
        {
            double hash = 0;
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++) { hash += this[i, j]; }
            }

            return (int) hash;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string seperator = "";
            builder.Append("{").Append(System.Environment.NewLine);
            for (int i = 0; i < Row; i++)
            {
                builder.Append("{");
                for (int j = 0; j < Column; j++)
                {
                    builder.Append(seperator).Append(this[i, j].ToString());
                    seperator = ",";
                }
                builder.Append("}").Append(System.Environment.NewLine);
                seperator = "";
            }
            builder.Append("}");

            return builder.ToString();
        }

        internal static Fraction[,] InitializeArray(int row, int column)
        {
            Fraction[,] newFraction = new Fraction[row, column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++) { newFraction[i, j] = 0.0; }
            }

            return newFraction;
        }

        public static explicit operator Vector.V4D(Matrix matrix)
        {
            if (matrix.Column == 1 && matrix.Row == 4)
            {
                return new Vector.V4D(matrix[0, 0], matrix[1, 0], matrix[2, 0], matrix[3, 0]);
            }
            else
            {
                throw new Exception("Matrix is not of type 4 x 1");
            }
        }

    }

    public class CMatrix
    {
        public int Row { get; internal set; }
        public int Column { get; internal set; }
        public Complex[,] Values { get; set; }

        public CMatrix(int row, int column)
        {
            Row = row;
            Column = column;
            Values = InitializeArray(row, column);
        }

        public CMatrix() { }

        public void AddValues(Complex[,] values)
        {
            Row = values.GetLength(0);
            Column = values.GetLength(1);
            Values = values;
        }

        public Complex this[int x, int y]
        {
            get { return Values[x, y]; }
            set { Values[x, y] = value; }
        }

        public static CMatrix operator -(CMatrix a, CMatrix b)
        {
            CMatrix tempMatrix = new CMatrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Row; j++) { tempMatrix[i, j] = a[i, j] - b[i, j]; }
            }

            return tempMatrix;
        }

        public static CMatrix operator -(CMatrix a)
        {
            CMatrix tempMatrix = new CMatrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = -a[i, j]; }
            }

            return tempMatrix;
        }

        public static bool operator !=(CMatrix a, CMatrix b) { return !(a == b); }

        public static bool operator ==(CMatrix a, CMatrix b)
        {
            if ((object)a == null && (object)b == null) { return true; }
            if ((object)a == null) { return false; }
            if ((object)b == null) { return false; }
            if (a.Row != b.Row || a.Column != b.Column) { return false; }

            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++)
                {
                    if (a[i, j] != b[i, j]) { return false; }
                }
            }

            return true;
        }

        public static CMatrix operator *(CMatrix a, CMatrix b)
        {
            if (a.Column == b.Row)
            {
                var tempMatrix = new CMatrix(a.Row, b.Column);
                //tempMatrix.AddValues(new Fraction[a.Row, b.Column]);
                for (int i = 0; i < a.Row; i++)
                {
                    for (int j = 0; j < b.Column; j++)
                    {
                        for (int r = 0; r < b.Row; r++) { tempMatrix[i, j] += (a[i, r] * b[r, j]); }
                    }
                }

                return tempMatrix;
            }
            else { throw new ArgumentException("Row/Column of the matrices do not seem to match!"); }
        }

        public static CMatrix operator *(CMatrix a, Fraction constant)
        {
            CMatrix tempMatrix = new CMatrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = constant * a[i, j]; }
            }

            return tempMatrix;
        }

        public static CMatrix operator *(Fraction constant, CMatrix a)
        {
            CMatrix tempMatrix = new CMatrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = constant * a[i, j]; }
            }

            return tempMatrix;
        }

        public static CMatrix operator /(CMatrix a, Fraction constant)
        {
            return a * (1 / constant);
        }

        public static CMatrix operator +(CMatrix a, CMatrix b)
        {
            CMatrix tempMatrix = new CMatrix(a.Row, a.Column);
            //tempMatrix.AddValues(new Fraction[a.Row, a.Column]);
            for (int i = 0; i < a.Row; i++)
            {
                for (int j = 0; j < a.Column; j++) { tempMatrix[i, j] = a[i, j] + b[i, j]; }
            }

            return tempMatrix;
        }

        public static void CopyTo(CMatrix a, CMatrix b)
        {
            for (int i = 0; i < b.Row; i++)
            {
                for (int j = 0; j < b.Column; j++) { b[i, j] = a[i, j]; }
            }
        }

        public CMatrix[] LUDecomposition()
        {
            int n = Row;
            CMatrix temp = new CMatrix(Row, Column);
            //temp.AddValues(new Fraction[Row, Column]);
            CopyTo(this, temp);
            CMatrix L = new CMatrix(n, n);
            CMatrix U = new CMatrix(n, n);
            //L.AddValues(new Fraction[n, n]);
            //U.AddValues(new Fraction[n, n]);
            for (int j = 0; j < Column; j++) { U[0, j] = temp[0, j]; }
            //for (int i = 0; i < n; i++) { U[i, i] = this[i, i]; }
            //for (int j = 1; j < Column; j++) { for (int i = 0; i < Row - 1; i++) { U[i, j] = this[i, j]; } }
            //for (int i = 0; i < n; i++) { U[i, i] = 0; }
            //for (int j = 1; j < Column; j++) { for (int i = 0; i < Row - 1; i++) { L[i, j] = 0; } }
            for (int i = 0; i < n; i++) { L[i, i] = 1; }

            for (int k = 0; k < n; k++)
            {
                U[k, k] = temp[k, k];
                for (int i = k + 1; i < n; i++)
                {
                    L[i, k] = (temp[i, k] / U[k, k]);
                    U[k, i] = temp[k, i];
                }

                for (int i = k + 1; i < n; i++)
                {
                    for (int j = k + 1; j < n; j++) { temp[i, j] = temp[i, j] - (L[i, k] * U[k, j]); }
                }
            }

            return new CMatrix[2] { L, U };
        }

        //public CMatrix[] LUPDecomposition()
        //{
        //    int n = Row;
        //    int[] piearray = new int[n];
        //    CMatrix temp = new CMatrix(n, n);
        //    //temp.AddValues(new Fraction[n, n]);
        //    CopyTo(this, temp);
        //    CMatrix P = new CMatrix(n, n);
        //    //P.AddValues(new Fraction[n, n]);
        //    int kprime = 0;

        //    for (int i = 0; i < n; i++) { piearray[i] = i; }

        //    for (int k = 0; k < n; k++)
        //    {
        //        Fraction p = 0;
        //        for (int i = k; i < n; i++)
        //        {
        //            if (Complex.Abs((double)temp[i, k]) > p) { p = Complex.Abs((double)temp[i, k]); kprime = i; }
        //        }
        //        if (p == (Fraction)0) { throw new Exception("Singular Matrix!"); }
        //        int tempvalueholder = piearray[kprime]; /* changed k to kprime */
        //        piearray[kprime] = piearray[k];
        //        piearray[k] = tempvalueholder;
        //        for (int i = 0; i < n; i++)
        //        {
        //            Complex tempvalueholder2 = temp[kprime, i]; /* changed k to kprime */
        //            temp[kprime, i] = temp[k, i];
        //            temp[k, i] = tempvalueholder2;
        //        }

        //        for (int i = k + 1; i < n; i++)
        //        {
        //            temp[i, k] = (temp[i, k] / temp[k, k]);
        //            for (int j = k + 1; j < n; j++) { temp[i, j] = temp[i, j] - (temp[i, k] * temp[k, j]); }
        //        }
        //    }

        //    int rcounter = 0;
        //    foreach (int cnumber in piearray) { P[rcounter, cnumber] = 1; rcounter++; }

        //    return new CMatrix[2] { P, temp };
        //}

        public Complex NDeterminant() /* Fix Bug */
        {
            if (Row == Column)
            {
                if (Row == 2) { return (this[0, 0] * this[1, 1]) - (this[0, 1] * this[1, 0]); }
                Complex result = 0.0;

                for (int i = 0; i < Row; i++)
                {
                    CMatrix tempMatrix = new CMatrix(Row - 1, Column - 1);
                    //tempMatrix.AddValues(new Fraction[Row - 1, Column - 1]);
                    int RowCount = 0;

                    for (int j = 0; j < Row; j++)
                    {
                        if (j != i)
                        {
                            for (int k = 1; k < Column; k++) { tempMatrix[RowCount, k - 1] = this[j, k]; }
                            RowCount++;
                        }

                    }

                    if (i % 2 == 0) { result += tempMatrix.NDeterminant(); }
                    else { result += tempMatrix.NDeterminant(); }
                }

                return result;
            }
            else { return 0; }
        }

        public Complex GDeterminant()
        {
            if (Row == Column)
            {
                CMatrix tempMatrix = LUDecomposition()[1];
                Complex result = 1.0;
                for (int i = 0; i < tempMatrix.Row; i++) { result *= tempMatrix[i, i]; }

                return result;
            }
            else { return 0; }
        }

        //public int Nullity()
        //{
        //    CMatrix temp = RREF();
        //    int counter = 0;
        //    for (int i = 0; i < Row; i++) { if (temp[i, i] == (Fraction)1) { counter++; } }

        //    return Row - counter;
        //}

        public static CMatrix Identity(int degree)
        {
            CMatrix newMatrix = new CMatrix(degree, degree);
            //newMatrix.AddValues(new Fraction[row,column]);
            for (int i = 0; i < degree; i++) { newMatrix[i, i] = 1.0; }

            return newMatrix;
        }

        //public CMatrix RREF()
        //{
        //    CMatrix tempMatrix = new CMatrix(Row, Column);
        //    //tempMatrix.AddValues(new Fraction[Row, Column]);
        //    CopyTo(this, tempMatrix);
        //    int lead = 0, rowCount = Row, columnCount = Column;
        //    for (int r = 0; r < rowCount; r++)
        //    {
        //        if (columnCount <= lead) break;
        //        int i = r;
        //        while (tempMatrix[i, lead] == (Fraction)0)
        //        {
        //            i++;
        //            if (i == rowCount)
        //            {
        //                i = r;
        //                lead++;
        //                if (columnCount == lead)
        //                {
        //                    lead--;
        //                    break;
        //                }
        //            }
        //        }
        //        for (int j = 0; j < columnCount; j++)
        //        {
        //            Complex temp = tempMatrix[r, j];
        //            tempMatrix[r, j] = tempMatrix[i, j];
        //            tempMatrix[i, j] = temp;
        //        }
        //        Complex div = tempMatrix[r, lead];
        //        if (div != (Fraction)0)
        //            for (int j = 0; j < columnCount; j++) tempMatrix[r, j] /= div;
        //        for (int j = 0; j < rowCount; j++)
        //        {
        //            if (j != r)
        //            {
        //                Complex sub = tempMatrix[j, lead];
        //                for (int k = 0; k < columnCount; k++) tempMatrix[j, k] -= (sub * tempMatrix[r, k]);
        //            }
        //        }
        //        lead++;
        //    }

        //    return tempMatrix;
        //}

        public CMatrix RetrieveColumn(int column)
        {
            CMatrix newMatrix = new CMatrix(Row, 1);
            CMatrix thisholder = new CMatrix(Row, Column);
            //thisholder.AddValues(new Fraction[Row, Column]);
            CopyTo(this, thisholder);
            //newMatrix.AddValues(new Fraction[Column, 1]);
            Complex[,] columnarray = new Complex[Row, 1];

            for (int i = 0; i < Row; i++) { columnarray[i, 0] = thisholder[i, column]; }
            newMatrix.AddValues(columnarray);

            return newMatrix;
        }

        public CMatrix RetrieveRow(int row)
        {
            CMatrix newMatrix = new CMatrix(1, Column);
            CMatrix thisholder = new CMatrix(Row, Column);
            //thisholder.AddValues(new Fraction[Row, Column]);
            CopyTo(this, thisholder);
            //newMatrix.AddValues(new Fraction[1, Column]);
            Complex[,] rowarray = new Complex[1, Column];

            for (int i = 0; i < Column; i++) { rowarray[0, i] = thisholder[row, i]; }
            newMatrix.AddValues(rowarray);

            return newMatrix;
        }

        //public bool IsSingular() { if (GDeterminant() == (Fraction)0) { return true; } else { return false; } }

        //public DLList<DLList<CMatrix>> RSpace()
        //{
        //    CMatrix temp = this.RREF();
        //    var returnlist = new DLList<DLList<CMatrix>>();
        //    returnlist.Append(new DLList<CMatrix>());
        //    returnlist.Append(new DLList<CMatrix>());

        //    for (int i = 0; i < Row; i++)
        //    {
        //        if (temp[i, i] == (Fraction)1)
        //        {
        //            //double[,] temparray = new double[1, Column];
        //            //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
        //            CMatrix insertionMatrix = temp.RetrieveRow(i);
        //            CMatrix tempMatrix = new CMatrix(1, Row);
        //            //tempMatrix.AddValues(new Fraction[1, Row]);
        //            CopyTo(insertionMatrix, tempMatrix);
        //            ((DLList<CMatrix>)returnlist[0]).Append(tempMatrix);
        //        }
        //        else if (temp[i, i] == (Fraction)0)
        //        {
        //            //double[,] temparray = new double[1, Column];
        //            //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
        //            CMatrix insertionMatrix = temp.RetrieveRow(i);
        //            CMatrix tempMatrix = new CMatrix(1, Row);
        //            //tempMatrix.AddValues(new Fraction[1, Row]);
        //            CopyTo(insertionMatrix, tempMatrix);
        //            ((DLList<CMatrix>)returnlist[1]).Append(insertionMatrix);
        //        }
        //    }

        //    return returnlist;
        //}

        //public DLList<DLList<CMatrix>> CSpace()
        //{
        //    CMatrix temp = RREF();
        //    var returnlist = new DLList<DLList<CMatrix>>();
        //    returnlist.Append(new DLList<CMatrix>());
        //    returnlist.Append(new DLList<CMatrix>());

        //    for (int i = 0; i < Row; i++)
        //    {
        //        if (temp[i, i] == (Fraction)1)
        //        {
        //            //double[,] temparray = new double[1, Column];
        //            //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
        //            CMatrix insertionMatrix = RetrieveColumn(i);
        //            CMatrix tempMatrix = new CMatrix(Row, 1);
        //            //tempMatrix.AddValues(new Fraction[Column, 1]);
        //            CopyTo(insertionMatrix, tempMatrix);
        //            ((DLList<CMatrix>)returnlist[0]).Append(tempMatrix);
        //        }
        //        else if (temp[i, i] == (Fraction)0)
        //        {
        //            //double[,] temparray = new double[1, Column];
        //            //for (int j = 0; j < Row; j++) { temparray[0, j] = temp[j, i]; }
        //            CMatrix insertionMatrix = RetrieveColumn(i);
        //            CMatrix tempMatrix = new CMatrix(Column, 1);
        //            //tempMatrix.AddValues(new Fraction[Column, 1]);
        //            CopyTo(insertionMatrix, tempMatrix);
        //            ((DLList<CMatrix>)returnlist[1]).Append(insertionMatrix);
        //        }
        //    }

        //    return returnlist;
        //}

        public CMatrix Transpose()
        {
            var tempValues = new CMatrix(Row, Column);
            //tempValues.AddValues(new Fraction[Row, Column]);

            for (int x = 0; x < Row; ++x)
            {
                for (int y = 0; y < Column; ++y) { tempValues[y, x] = Values[x, y]; }
            }

            return tempValues;
        }

        public override bool Equals(object obj)
        {
            var tempobj = obj as CMatrix;
            return (tempobj != null) && (this == tempobj);
        }

        //public override int GetHashCode()
        //{
        //    double hash = 0;
        //    for (int i = 0; i < Row; i++)
        //    {
        //        for (int j = 0; j < Column; j++) { hash += this[i, j]; }
        //    }

        //    return (int)hash;
        //}

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string seperator = "";
            builder.Append("{").Append(System.Environment.NewLine);
            for (int i = 0; i < Row; i++)
            {
                builder.Append("{");
                for (int j = 0; j < Column; j++)
                {
                    builder.Append(seperator).Append(this[i, j].ToString());
                    seperator = ",";
                }
                builder.Append("}").Append(System.Environment.NewLine);
                seperator = "";
            }
            builder.Append("}");

            return builder.ToString();
        }

        internal static Complex[,] InitializeArray(int row, int column)
        {
            Complex[,] newFraction = new Complex[row, column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++) { newFraction[i, j] = 0.0; }
            }

            return newFraction;
        }

        public static explicit operator Vector.CVND(CMatrix matrix)
        {
            if (matrix.Column == 1)
            {
                Complex[] subvalue = new Complex[matrix.Row];
                for (int i = 0; i < matrix.Row; i++)
                {
                    subvalue[i] = matrix.Values[i, 0];
                }
                return new Vector.CVND(subvalue);
            }
            else
            {
                throw new Exception("Matrix is not of type N x 1");
            }
        }
    }
}
