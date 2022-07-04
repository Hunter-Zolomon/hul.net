using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HUL
{
    public class Fraction
    {
        public Int128 Numerator { get; set; }
        public Int128 Denominator { get; set; }

        public Fraction(Int128 numerator, Int128 denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public Fraction(long numerator, long denominator)
        {
            Numerator = numerator; //(Int128)
            Denominator = denominator; //(Int128)
        }

        public Fraction(double numerator, double denominator)
        {
            while (numerator != Math.Round(numerator, MidpointRounding.AwayFromZero)
                   || denominator != Math.Round(denominator, MidpointRounding.AwayFromZero))
            //while (Math.Abs(numerator % 1) <= (Double.Epsilon * 100) || Math.Abs(denominator % 1) <= (Double.Epsilon * 100))
            {
                numerator *= 10;
                denominator *= 10;
            }
            Numerator = (Int128)numerator;
            Denominator = (Int128)denominator;
            if (denominator == int.MinValue)
                return;
            Reduce();
        }

        public Fraction(decimal numerator, decimal denominator)
        {
            while (numerator != Math.Round(numerator, MidpointRounding.AwayFromZero)
                   || denominator != Math.Round(denominator, MidpointRounding.AwayFromZero))
            {
                numerator *= 10;
                denominator *= 10;
            }
            Numerator = (Int128)numerator;
            Denominator = (Int128)denominator;
            Reduce();
        }

        public Fraction(float numerator, float denominator)
        {
            while (numerator != Math.Round(numerator, MidpointRounding.AwayFromZero)
                   || denominator != Math.Round(denominator, MidpointRounding.AwayFromZero))
            {
                numerator *= 10;
                denominator *= 10;
            }
            Numerator = (Int128)numerator;
            Denominator = (Int128)denominator;
            if (denominator == int.MinValue)
                return;
            Reduce();
        }

        public static implicit operator decimal(Fraction fraction)
        {
            return ((decimal)fraction.Numerator / (decimal)fraction.Denominator);
        }

        public static implicit operator double(Fraction fraction)
        {
            return ((double)fraction.Numerator / (double)fraction.Denominator);
        }

        public static implicit operator float(Fraction fraction)
        {
            return ((float)fraction.Numerator / (float)fraction.Denominator);
        }

        public static implicit operator int(Fraction fraction)
        {
            return (int)(fraction.Numerator / fraction.Denominator);
        }

        public static implicit operator Fraction(double fraction)
        {
            return new Fraction(fraction, 1.0);
        }

        public static implicit operator Fraction(decimal fraction)
        {
            return new Fraction(fraction, 1.0m);
        }

        public static implicit operator Fraction(float fraction)
        {
            return new Fraction(fraction, 1.0);
        }

        public static implicit operator Fraction(long fraction)
        {
            return new Fraction(fraction, 1);
        }

        public static implicit operator Fraction(int fraction)
        {
            return new Fraction(fraction, 1);
        }

        public static implicit operator Fraction(uint fraction)
        {
            return new Fraction((int)fraction, 1);
        }

        public static implicit operator string(Fraction fraction)
        {
            return fraction.ToString();
        }

        public static Fraction operator -(Fraction first, Fraction second)
        {
            var val1 = new Fraction(first.Numerator * (Int128)second.Denominator, first.Denominator * second.Denominator);
            var val2 = new Fraction(second.Numerator * (Int128)first.Denominator, second.Denominator * first.Denominator);
            var result = new Fraction(val1.Numerator - val2.Numerator, val1.Denominator);
            result.Reduce();
            return result;
        }

        public static Fraction operator -(Fraction first)
        {
            return new Fraction(-first.Numerator, first.Denominator);
        }

        public static Fraction operator *(Fraction first, Fraction second)
        {
            var result = new Fraction(first.Numerator * second.Numerator, first.Denominator * second.Denominator);
            result.Reduce();
            return result;
        }

        public static Complex operator *(Fraction first, Complex second)
        {
            var result = new Complex(second.Real * (double)first, second.Imaginary * (double)first);
            return result;
        }

        public static Fraction operator /(Fraction first, Fraction second)
        {
            return first * second.Inverse();
        }

        public static Fraction operator +(Fraction first, Fraction second)
        {
            var val1 = new Fraction(first.Numerator * (Int128)second.Denominator, first.Denominator * second.Denominator);
            var val2 = new Fraction(second.Numerator * (Int128)first.Denominator, second.Denominator * first.Denominator);
            var result = new Fraction(val1.Numerator + val2.Numerator, val1.Denominator);
            result.Reduce();
            return result;
        }

        public static bool operator !=(Fraction first, Fraction second)
        {
            return !(first == second);
        }

        public static bool operator !=(Fraction first, double second)
        {
            return !(first == second);
        }

        public static bool operator !=(double first, Fraction second)
        {
            return !(first == second);
        }

        public static bool operator ==(Fraction first, Fraction second)
        {
            return first.Equals(second);
        }

        public static bool operator ==(Fraction first, double second)
        {
            return first.Equals(second);
        }

        public static bool operator ==(double first, Fraction second)
        {
            return second.Equals(first);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Fraction;
            if (((object)other) == null)
                return false;
            decimal val1 = this;
            decimal val2 = other;
            return val1 == val2;
        }

        public override int GetHashCode()
        {
            return Numerator.GetHashCode() % Denominator.GetHashCode();
        }

        public override string ToString()
        {
            if (Numerator == 0) { return $"{0}"; }
            else if (Denominator == 1) { return $"{Numerator}"; }
            else { return $"{Numerator}/{Denominator}"; }
        }

        public Fraction Inverse()
        {
            return new Fraction((Int128)Denominator, Numerator);
        }

        public Int128 GCD(Int128 a, Int128 b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b) { a %= b; }
                else { b %= a; }
            }

            if (a == 0) { return b; }
            else { return a; }
        }

        public void Reduce()
        {
            if ((Numerator < 0 && Denominator < 0) || (Numerator > 0 && Denominator < 0))
            {
                Numerator = Numerator * -1;
                Denominator = Denominator * -1;
            }
            //if (Numerator / 1 < 0) { Numerator = Numerator * -1; }
            //if (Denominator / 1 < 0) { Denominator = Denominator * -1; }
            //var gcd = GCD(Numerator, Denominator); !!!DEPRECATED!!!
            Int128 gcd = Int128.GreatestCommonDivisor(Numerator, Denominator);
            if (gcd != 0)
            {
                Numerator /= gcd;
                Denominator /= gcd;
            }
        }
    }

    public class CFraction
    {
        public Complex complexnumerator { get; set; }
        public Complex complexdenominator { get; set; }

        public CFraction(Complex cnum1, Complex cnum2)
        {
            complexnumerator = cnum1;
            complexdenominator = cnum2;
        }

        public static CFraction operator -(CFraction fraction1, CFraction fraction2)
        {
            var tempfraction1 = Reduce(fraction1);
            var tempfraction2 = Reduce(fraction2);
            var val1 = new CFraction(tempfraction1.complexnumerator * tempfraction2.complexdenominator, tempfraction1.complexdenominator * tempfraction2.complexdenominator);
            var val2 = new CFraction(tempfraction2.complexnumerator * tempfraction1.complexdenominator, tempfraction2.complexdenominator * tempfraction1.complexdenominator);
            var result = new CFraction(val1.complexnumerator - val2.complexnumerator, val1.complexdenominator);
            return result;
        }

        public static CFraction operator -(CFraction fraction, bool reduce)
        {
            if (reduce)
            {
                var tempfraction = Reduce(fraction);
                return new CFraction(-tempfraction.complexnumerator, tempfraction.complexdenominator);
            }
            else
            {
                return new CFraction(-fraction.complexnumerator, fraction.complexdenominator);
            }
        }

        public static CFraction operator +(CFraction fraction1, CFraction fraction2)
        {
            var tempfraction1 = Reduce(fraction1);
            var tempfraction2 = Reduce(fraction2);
            var val1 = new CFraction(tempfraction1.complexnumerator * tempfraction2.complexdenominator, tempfraction1.complexdenominator * tempfraction2.complexdenominator);
            var val2 = new CFraction(tempfraction2.complexnumerator * tempfraction1.complexdenominator, tempfraction2.complexdenominator * tempfraction1.complexdenominator);
            var result = new CFraction(val1.complexnumerator + val2.complexnumerator, val1.complexdenominator);
            return result;
        }

        public static CFraction operator *(CFraction fraction1, CFraction fraction2)
        {
            CFraction resultfraction = new CFraction(fraction1.complexnumerator * fraction2.complexnumerator, fraction1.complexdenominator * fraction2.complexdenominator);
            return Reduce(resultfraction);
        }

        public static CFraction operator /(CFraction fraction1, CFraction fraction2)
        {
            return fraction1 * fraction2.Inverse();
        }

        public CFraction Inverse()
        {
            return new CFraction(complexdenominator, complexnumerator);
        }

        public static CFraction Reduce(CFraction fraction)
        {
            var numerator = fraction.complexnumerator * Complex.Conjugate(fraction.complexdenominator);
            var denominator = fraction.complexdenominator * Complex.Conjugate(fraction.complexdenominator);
            return new CFraction(numerator / denominator.Real, 1);
        }

        public override string ToString()
        {
            if (complexnumerator == 0) { return $"{0}"; }
            else if (complexdenominator == 1) { return $"{complexnumerator.Real + "+" + complexnumerator.Imaginary + "i"}"; }
            else { return $"{complexnumerator.Real + "+" + complexnumerator.Imaginary + "i"}/{complexdenominator.Real + "+" + complexdenominator.Imaginary + "i"}"; }
        }

        public override int GetHashCode()
        {
            return complexnumerator.GetHashCode() % complexdenominator.GetHashCode();
        }
    }
}
