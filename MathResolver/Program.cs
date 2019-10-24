using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathResolver.Structure;

namespace MathResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine($"Converting..");
            //var convResult = Measure.It(TestConverting, out var conTime);
            //Console.WriteLine($"Converting Result: {convResult.Item2}/{convResult.Item1} ({(double)convResult.Item2 / convResult.Item1:P})");
            //Console.WriteLine($"Time: {conTime:c}.");
            //Console.WriteLine("Adding..");
            //Measure.It(TestAdding, out var addTime);
            //Console.WriteLine($"Time: {addTime:c}.");
            //Console.WriteLine("Subbing..");
            //Measure.It(TestSubbing, out var subTime);
            //Console.WriteLine($"Time: {subTime:c}.");
            //Console.WriteLine($"LargeAdding..");
            //Measure.It(LongAdd, out var longTime);
            //Console.WriteLine($"Time: {longTime:c}.");
            //var term = "2 + 3 * (1 + 2 * (4 + 3 * 2)) - (1 - 2) + 1";
            //Console.WriteLine($"Create Resolver with Term \"{term}\"");
            //var resolver = new BasicResolver(term);
            //Console.WriteLine($"Resolve..");
            //var result = resolver.Resolve();
            //Console.WriteLine($"Result: {result}.");

            var input = string.Empty;
            TimeSpan time;
            do
            {
                var color = Console.ForegroundColor;

                Console.Write("Term: ");
                input = Console.ReadLine();

                try
                {
                    Console.WriteLine($"Creating Resolver..");
                    var resolver = Measure.It(() => new BasicResolver(input), out time);
                    Console.WriteLine($"Created in: {time:c}!");

                    if (resolver.Variables.Any())
                    {
                        Console.WriteLine("Insert Values for Variables.");
                        var keys = resolver.Variables.Keys.ToArray();
                        foreach (var key in keys)
                        {
                            Console.Write($"\"{key}\": ");
                            var value = Console.ReadLine();
                            if(decimal.TryParse(value, out var dec))
                            {
                                resolver.Variables[key] = dec;
                            }
                        }
                    }

                    Console.WriteLine($"Resolve..");
                    var result = Measure.It(resolver.Resolve, out time);
                    Console.WriteLine($"Resolved in: {time:c}!");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Result: {result}");
                }
                catch (InvalidExpressionException inv)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Expression Error in: \"{inv.Expression}\"!");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.ForegroundColor = color;
            } while (input != string.Empty);


            //Console.ReadLine();
        }

        static (int, int) TestConverting()
        {
            var random = new Random();
            var count = 0;
            var valid = 0;
            for (int i = 0; i < 10000; i++)
            {
                var type = random.Next(2);
                var sign = random.Next(2);
                var signString = sign == 1 ? "-" : "+";
                var startZeros = random.Next(10);
                var startZeroString = new string('0', startZeros);
                var endZeros = random.Next(10);
                var endZeroString = new string('0', endZeros);
                if (type == 0)
                {
                    var value = random.Next();
                    var result = Validate(value.ToString(), startZeroString, "", signString);
                    if (result) valid++;
                }
                else
                {
                    var value = random.NextDouble() * random.Next();
                    var result = Validate(value.ToString(), startZeroString, endZeroString, signString);
                    if (result) valid++;
                }

                count++;
            }

            return (count, valid);
            // Console.WriteLine($"Checks: {count}. Valid: {valid}. Invalid: {count - valid}. Rate: {(double)valid / count:P}");
        }
        static bool Validate(string value, string startZeros, string endZeros, string sign)
        {
            var v = $"{sign}{startZeros}{value}{endZeros}";
            //Console.WriteLine($"Try:        \"{v}\"");
            var number = new Number(v);
            var result = number.ToString();
            //Console.WriteLine($"Result:     \"{result}\"");
            //Console.WriteLine($"CompareInternal to: \"{sign}{value}\"");
            var isEqual = ((sign == "+" ? "" : sign) + value) == result;

            //var color = Console.ForegroundColor;
            //if (isEqual)
            //{
            //    Console.WriteLine($"Correct.");
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.DarkRed;
            //    Console.WriteLine($"Incorrect!");
            //}
            //Console.ForegroundColor = color;

            return isEqual;
        }

        static Number GenerateRandomNumber(Random random, out object value, object testValue=null)
        {
            if (testValue != null)
            {
                value = testValue;
                return new Number(testValue);
            }

            var type = random.Next(2);
            var sign = random.Next(3) == 1 ? -1 : 1;
            if (type == 0)
            {
                var v = random.Next(1001) * sign;
                value = v;
                return new Number(v);
            }
            else
            {
                var v = Math.Round(random.NextDouble() * random.Next(100) * sign, random.Next(2, 7));
                value = v;
                return new Number(v);
            }
        }

        static void TestAdding()
        {
            var input = string.Empty;
            var random = new Random(0);
            var maxIts = 10000;
            do
            {
                var v1 = GenerateRandomNumber(random, out var vo1);
               // Console.WriteLine($" First Value: \"{v1}\". Original: \"{vo1}\"");
                var v2 = GenerateRandomNumber(random, out var vo2);
               // Console.WriteLine($"Second Value: \"{v2}\". Original: \"{vo2}\"");

                var r = v1 + v2; //Number.AddInternal(v1, v2);
                var ro = Add(vo1, vo2);

                //Console.WriteLine($"  Add Result: \"{r}\". Original: \"{ro}\"");
                //input = Console.ReadLine();
            } while (input == string.Empty && maxIts-- > 0);
        }

        static object Add(object n1, object n2)
        {
            if (n1 is double d1 && n2 is double d2)
            {
                return d1 + d2;
            }

            if (n1 is int i1 && n2 is int i2)
            {
                return i1 + i2;
            }

            if (n1 is double dn1 && n2 is int id2)
            {
                return dn1 + id2;
            }

            if (n1 is int id1 && n2 is double dn2)
            {
                return id1 + dn2;
            }

            return null;
        }


        static void TestSubbing()
        {
            var input = string.Empty;
            var random = new Random(0);
            var maxIts = 10000;
            do
            {
                var v1 = GenerateRandomNumber(random, out var vo1);
                //Console.WriteLine($" First Value: \"{v1}\". Original: \"{vo1}\"");
                var v2 = GenerateRandomNumber(random, out var vo2);
                //Console.WriteLine($"Second Value: \"{v2}\". Original: \"{vo2}\"");

                //var c = Number.CompareInternal(v1, v2);
                //var c2 = Number.CompareInternal(v1, new Number(v1.ToString()));
                //var c3 = Number.CompareInternal(v2, new Number(v2.ToString()));
                //Console.WriteLine($"CompareInternal: {c} (First: {c2}, Second: {c3})");

                var r = v1 - v2;//Number.SubInternal(v1, v2);
                var ro = Sub(vo1, vo2);

                //Console.WriteLine($"  Sub Result: \"{r}\". Original: \"{ro}\"");
                //input = Console.ReadLine();
            } while (input == string.Empty && maxIts-- > 0);
        }

        static object Sub(object n1, object n2)
        {
            if (n1 is double d1 && n2 is double d2)
            {
                return d1 - d2;
            }

            if (n1 is int i1 && n2 is int i2)
            {
                return i1 - i2;
            }

            if (n1 is double dn1 && n2 is int id2)
            {
                return dn1 - id2;
            }

            if (n1 is int id1 && n2 is double dn2)
            {
                return id1 - dn2;
            }

            return null;
        }

        static void LongAdd()
        {
            var random = new Random(0);
            var n1 = GenerateLargeNumber(random, 10_000_000, 10_000_000);
            var n2 = GenerateLargeNumber(random, 10_000_000, 10_000_000);
            var n3 = n1 + n2;
        }

        static Number GenerateLargeNumber(Random random, long integer, long decimals)
        {
            var blockSize = 1_000_000;
            var block =  Math.Min(integer, blockSize);
            var s = new StringBuilder((int) block);
            for (var i = 0; i < block; i++)
            {
                var z = random.Next(0, 10);
                s.Append(z);
            }

            var b = s.ToString();

            var result = string.Empty;
            for (var i = 0; i < integer / block; i++)
            {
                result += b;
            }

            if (decimals > 0)
            {
                result += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                block = (int)Math.Min(decimals, blockSize);
                s = new StringBuilder((int) block);
                for (var i = 0; i < block; i++)
                {
                    var z = random.Next(0, 10);
                    s.Append(z);
                }

                b = s.ToString();

                for (var i = 0; i < decimals / block; i++)
                {
                    result += b;
                }
            }

            return new Number(result);
        }
    }
}
