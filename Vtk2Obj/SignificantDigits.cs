using System;
using System.Text;

namespace IsoSurfaceToObj {
    /// <summary>
    /// https://stackoverflow.com/questions/374316/round-a-double-to-x-significant-figures
    /// </summary>
    public static class SignificantDigits {
        private static readonly string DecimalSeparator;

        static SignificantDigits() {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            DecimalSeparator = ci.NumberFormat.NumberDecimalSeparator;
        }

        /// <summary>
        /// Format a double to a given number of significant digits.
        /// </summary>
        /// <example>
        /// 0.086 -> "0.09" (digits = 1)
        /// 0.00030908 -> "0.00031" (digits = 2)
        /// 1239451.0 -> "1240000" (digits = 3)
        /// 5084611353.0 -> "5085000000" (digits = 4)
        /// 0.00000000000000000846113537656557 -> "0.00000000000000000846114" (digits = 6)
        /// 50.8437 -> "50.84" (digits = 4)
        /// 50.846 -> "50.85" (digits = 4)
        /// 990.0 -> "1000" (digits = 1)
        /// -5488.0 -> "-5000" (digits = 1)
        /// -990.0 -> "-1000" (digits = 1)
        /// 0.0000789 -> "0.000079" (digits = 2)
        /// </example>
        public static string ToPreciseString(this double number, int digits = 8, bool showTrailingZeros = false, bool alwaysShowDecimalSeparator = false) {
            if (Double.IsNaN(number) ||
                Double.IsInfinity(number)) {
                return number.ToString();
            }

            string sSign = "";
            string sBefore = "0"; // Before the decimal separator
            string sAfter = ""; // After the decimal separator

            if (number != 0d) {
                if (digits < 1) {
                    throw new ArgumentException("The digits parameter must be greater than zero.");
                }

                if (number < 0d) {
                    sSign = "-";
                    number = Math.Abs(number);
                }

                // Use scientific formatting as an intermediate step
                string sFormatString = "{0:" + new String('#', digits) + "E0}";
                string sScientific = String.Format(sFormatString, number);

                string sSignificand = sScientific.Substring(0, digits);
                int exponent = Int32.Parse(sScientific.Substring(digits + 1));
                // (the significand now already contains the requested number of digits with no decimal separator in it)

                StringBuilder sFractionalBreakup = new StringBuilder(sSignificand);

                if (!showTrailingZeros) {
                    while (sFractionalBreakup[sFractionalBreakup.Length - 1] == '0') {
                        sFractionalBreakup.Length--;
                        exponent++;
                    }
                }

                // Place decimal separator (insert zeros if necessary)

                int separatorPosition = 0;

                if ((sFractionalBreakup.Length + exponent) < 1) {
                    sFractionalBreakup.Insert(0, "0", 1 - sFractionalBreakup.Length - exponent);
                    separatorPosition = 1;
                } else if (exponent > 0) {
                    sFractionalBreakup.Append('0', exponent);
                    separatorPosition = sFractionalBreakup.Length;
                } else {
                    separatorPosition = sFractionalBreakup.Length + exponent;
                }

                sBefore = sFractionalBreakup.ToString();

                if (separatorPosition < sBefore.Length) {
                    sAfter = sBefore.Substring(separatorPosition);
                    sBefore = sBefore.Remove(separatorPosition);
                }
            }

            string sReturnValue = sSign + sBefore;

            if (sAfter == "") {
                if (alwaysShowDecimalSeparator) {
                    sReturnValue += DecimalSeparator + "0";
                }
            } else {
                sReturnValue += DecimalSeparator + sAfter;
            }

            return sReturnValue.Substring(0, Math.Min(sReturnValue.Length, digits));
        }
    }
}