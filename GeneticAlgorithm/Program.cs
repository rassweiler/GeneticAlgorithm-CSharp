using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace GeneticAlgorithm
{
    class DNA
    {
        public int targetlength;
        public float mutationRate;
        public string targetText;
        public char[] genesArray;
        public float fitness { get; set; }

        public DNA(float mutation, string target)
        {
            targetText = target;
            mutationRate = mutation;
            targetlength = target.Length;
            genesArray = new char[targetlength];
        }

        /* Clone dna */
        public DNA(DNA dna)
        {
        	fitness = dna.fitness;
        	genesArray = dna.genesArray;
        	targetText = dna.targetText;
        	mutationRate = dna.mutationRate;
        	targetlength = dna.targetlength;
        }

        /* Create Child */
        public DNA(DNA p1, DNA p2, Random rand)
        {
        	targetText = p1.targetText;
        	mutationRate = p1.mutationRate;
        	targetlength = p1.targetlength;
        	genesArray = new char[targetlength];
        	for(int i = 0; i < targetlength; ++i){
        		if(rand.Next(2) == 1)
        		{
        			genesArray[i] = p1.genesArray[i];
        		}
        		else
        		{
        			genesArray[i] = p2.genesArray[i];
        		}
        	}
        	MutateGenes(rand);
        	CalculateFitness();
        }

        public void InitGenes(Random rand)
        {
            for (int i = 0; i < targetlength; ++i)
            {
                int character = rand.Next(32, 127);
                genesArray[i] = Convert.ToChar(character);
            }
        }

        public void MutateGenes(Random rand)
        {
            for (int i = 0; i < targetlength; ++i)
            {
                if(rand.NextDouble() < mutationRate)
                {
                	int character = rand.Next(32, 127);
                	genesArray[i] = Convert.ToChar(character);
                }
            }
        }

        public void PrintGenes()
        {
            Console.WriteLine(genesArray);
        }

        public void CalculateFitness()
        {
            float score = 0.0F;
            for (int i = 0; i < targetlength; ++i)
            {
                if(genesArray[i] == targetText[i])
                {
                    score += 1;
                }
            }
            fitness = score / targetlength;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            /* Variables */
            int populationSize = 1000;
            float mutationRate = 0.01F;
            string targetText = "Yeeeeeah boyyyyee!";
            List<DNA> populationList = new List<DNA>();
            List<DNA> matingList = new List<DNA>();
            Tuple<string, float> highestDNA;
            int generation = 0;
            Random random = new Random();

            /* Collect settings */
            Console.WriteLine("Genetic Algorithm C# Edition.\nKyle Rassweiler 2016\n-------------------------------\n\n");
            Console.WriteLine("Select Population Size(5000):");
            populationSize = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Select Mutation Rate(0.01):");
            mutationRate = float.Parse(Console.ReadLine());
            Console.WriteLine("Select Target Text:");
            targetText = Console.ReadLine();
            string settings = string.Format("Settings: {0}, {1}, ", populationSize, mutationRate);
            Console.WriteLine(settings + targetText);

            /* Setup starting generation */
            InitScenario(populationSize, mutationRate,targetText, populationList, random);
            highestDNA = GetHighestFitness(populationList);
            Console.WriteLine("Generation: {0}", generation);
            Console.WriteLine("Highest Fitness: {0}, Gene: {1}",highestDNA.Item2, highestDNA.Item1);
            
            /* Circle Of Life */
            while (highestDNA.Item2 < 1.0F)
            {
                generation += 1;
                matingList = FillMatingPool(populationList, matingList);
                populationList =RebuildPopulation(populationList, matingList, random);
                highestDNA = GetHighestFitness(populationList);
                Console.WriteLine("Generation: {0}", generation);
            	Console.WriteLine("Highest Fitness: {0}, Gene: {1}",highestDNA.Item2, highestDNA.Item1);
            }
            Console.ReadKey();
        }

        private static void InitScenario(int size, float mutation, string target, List<DNA> population, Random random)
        {
            for (int i = 0; i < size; ++i)
            {
                population.Add(new DNA(mutation, target));
                population[i].InitGenes(random);
                population[i].CalculateFitness();
            }
        }

        private static List<DNA> FillMatingPool(List<DNA> population, List<DNA> pool)
        {
        	pool.Clear();
        	int pSize = population.Count();
        	for(int i = 0; i < pSize; ++i)
        	{
        		int n = (int)(population[i].fitness*100);
        		for(int o = 0; o < n; ++o)
        		{
        			pool.Add(new DNA(population[i]));
        		}
        	}
        	return pool;
        }

        private static List<DNA> RebuildPopulation(List<DNA> population, List<DNA> pool, Random rand)
        {
        	int pSize = population.Count();
        	int mSize = pool.Count();
        	population.Clear();
        	for(int i = 0; i < pSize; ++i)
        	{
        		DNA p1 = pool[rand.Next(mSize)];
        		DNA p2 = pool[rand.Next(mSize)];
        		while(p2 == p1)
        		{
        			p2 = pool[rand.Next(mSize)];
        		}
        		population.Add(new DNA(p1, p2, rand));
        	}
        	return population;
        }

        private static Tuple<string, float> GetHighestFitness(List<DNA> population)
        {
            int size = population.Count;
            float highest = 0.0F;
            float current = 0.0F;
            int index = 0;
            for (int i = 0; i < size; ++i)
            {
                current = population[i].fitness;
                if(current > highest)
                {
                    highest = current;
                    index = i;
                }
            }
            return Tuple.Create(new string(population[index].genesArray), highest);
        }
    }
}
