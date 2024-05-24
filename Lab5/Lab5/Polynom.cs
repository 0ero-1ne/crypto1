namespace Lab5
{
    class Polynom
    {
        readonly private int degree;
        readonly private List<int> bites;
        public Polynom(int degree, string bites)
        {
            this.degree = degree;
            this.bites = new List<int>();

            foreach (char bit in bites)
            {
                this.bites.Add(Int32.Parse(bit.ToString()));
            }
            this.bites = RemoveLeadingZeroes(this.bites);
        } // Constructor 1: degree and bites as string

        public Polynom(int[] bites)
        {
            this.bites = RemoveLeadingZeroes(bites.ToList());
            degree = this.bites.Count - 1;
        } // Constructor 2: int array of bites

        public Polynom(int degree) // Constructor 3: degree
        {
            this.degree = degree;
            this.bites = new List<int>();

            for (int i = degree; i >= 0; i--)
            {
                if (i == degree)
                {
                    this.bites.Add(1);
                }
                else
                {
                    this.bites.Add(0);
                }
            }

        }

        public Polynom(List<int> bites)
        {
            this.bites = bites;
            this.degree = bites.Count - 1;
        } // Constructor 4: bites

        // returns cortege with 2 items: quotinent and remainder polynoms
        public (Polynom, Polynom) DivideByPolynom(Polynom polynom)
        {
            if (degree < polynom.degree)
            {
                throw new Exception("Error: The degree of the divisor is more than the degree of the dividend");
            }

            List<int> quotient = new();
            List<int> remainder = new();
            int counter = 0;

            while (counter < bites.Count)
            {
                remainder.Add(bites[counter]);

                if (remainder.Count == polynom.degree + 1)
                {
                    remainder = RemoveLeadingZeroes(remainder);
                }

                if (remainder.Count == polynom.degree + 1)
                {
                    quotient.Add(1);

                    for (int i = 0; i < remainder.Count; i++)
                    {
                        remainder[i] = remainder[i] ^ polynom.bites[i];
                    }

                    remainder = RemoveLeadingZeroes(remainder);
                }
                else
                {
                    if (quotient.Count != 0)
                    {
                        quotient.Add(0);
                    } 
                }

                counter++;
            }
            
            return (new Polynom(quotient.ToArray()), new Polynom(remainder.ToArray()));
        }

        /*
         * The method multiplies the original polynomial
         * by a polynomial with one term to the power of the polynomial itself.
         * returns Polynom
         */
        public Polynom MultiplyPolynomByPolynom(Polynom polynom)
        {
            List<int> result = bites;
            List<int> additionalZeroes = new(new int[polynom.degree]);
            return new Polynom(result.Concat(additionalZeroes).ToArray());
        }

        public override string ToString()
        {
            string result = "";
            bites.ForEach(item => result += item.ToString());
            return result;
        }
        
        private List<int> RemoveLeadingZeroes(List<int> list)
        {
            List<int> result = new();
            list.ForEach(item =>
            {
                if (result.Count == 0)
                {
                    if (item == 1)
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    result.Add(item);
                }
            });

            return result;
        }

        public List<int> GetBites() => this.bites;
    }
}
