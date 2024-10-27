using System;
using System.Collections.Generic;
using System.Linq;

class Block
{
    public int Height { get; private set; }

    public Block(int height)
    {
        Height = height;
    }
}

class Wall
{
    public List<Block> Blocks { get; private set; }

    public Wall(List<Block> blocks)
    {
        Blocks = new List<Block>(blocks);
    }

    /*public double Fitness()
    {
        int n = Blocks.Count;
        double score = 0.0;

        for (int i = 0; i < n / 2; i++)
        {
            int left = Blocks[i].Height;
            int right = Blocks[n - 1 - i].Height;
            int centerLeft = Blocks[n / 2 - 1 - i].Height;
            int centerRight = Blocks[n / 2 + i].Height;

            score += (centerLeft + centerRight) / 2.0 - (left + right) / 2.0;
        }

        return score;
    }*/
    public double Fitness()
    {
        int n = Blocks.Count;
        double score = 0.0;

        // Идеальная высота по краям и центру
        int midIndex = n / 2;

        int previousHeight = Blocks[0].Height;

        for (int i = 1; i < midIndex; i++)
        {
            if (Blocks[i].Height > previousHeight)
            {
                score++;
                previousHeight = Blocks[i].Height;
            } 
            else
            {
                break;
            }
        }

        previousHeight = Blocks[n-1].Height;

        for (int i = n-2; i >= midIndex; i--)
        {
            if (Blocks[i].Height > previousHeight)
            {
                score++;
                previousHeight = Blocks[i].Height;
            } 
            else
            {
                break;
            }
        }


        return score;
    }

}

class GeneticAlgorithm
{
    private List<Wall> population;
    private Random random = new Random();
    private int populationSize;
    private int numberOfGenerations;
    private double mutationRate;

    public GeneticAlgorithm(List<Block> initialBlocks, int populationSize, int numberOfGenerations, double mutationRate)
    {
        this.populationSize = populationSize;
        this.numberOfGenerations = numberOfGenerations;
        this.mutationRate = mutationRate;
        population = InitializePopulation(initialBlocks);
    }

    private List<Wall> InitializePopulation(List<Block> initialBlocks)
    {
        var population = new List<Wall>();

        for (int i = 0; i < populationSize; i++)
        {
            var shuffledBlocks = initialBlocks.OrderBy(x => random.Next()).ToList();
            population.Add(new Wall(shuffledBlocks));
        }

        return population;
    }

    public Wall Run()
    {
        Wall currentBest = null;

        for (int generation = 0; generation < numberOfGenerations; generation++)
        {
            population = population.OrderByDescending(w => w.Fitness()).ToList();
            var newPopulation = new List<Wall>();

            for (int i = 0; i < populationSize / 2; i++)
            {
                var parent1 = SelectParent();
                var parent2 = SelectParent();

                var offspring1 = Crossover(parent1, parent2);
                var offspring2 = Crossover(parent2, parent1);

                newPopulation.Add(Mutate(offspring1));
                newPopulation.Add(Mutate(offspring2));
            }

            population = newPopulation;

            currentBest = population.OrderByDescending(w => w.Fitness()).First();
            string bloks = "";
            foreach (var block in currentBest.Blocks)
            {
                bloks+= block.Height + " ";
            }
            Console.WriteLine($"Generation: {generation}; Blocks: {bloks} Best fitness: {currentBest.Fitness()}");
            if (currentBest.Fitness() == 8)
            {
                
                return currentBest;
            }
        }

        Console.WriteLine("Found best ");
        return currentBest;
    }

    private Wall SelectParent()
    {
        int tournamentSize = 3;
        var selected = new List<Wall>();

        for (int i = 0; i < tournamentSize; i++)
        {
            int randomIndex = random.Next(populationSize);
            selected.Add(population[randomIndex]);
        }

        return selected.OrderByDescending(w => w.Fitness()).First();
    }

    private Wall Crossover(Wall parent1, Wall parent2)
    {
        int n = parent1.Blocks.Count;
        Block[] childBlocks = new Block[n];

        int start = random.Next(n);
        int end = random.Next(start, n);

        for (int i = start; i < end; i++)
        {
            childBlocks[i] = parent1.Blocks[i];
        }

        int parent2Index = 0;

        for (int i = 0; i < n; i++)
        {
            if (childBlocks[i] == null)
            {
                while (childBlocks.Any(b => b != null && b.Height == parent2.Blocks[parent2Index].Height))
                {
                    parent2Index++;
                }

                childBlocks[i] = parent2.Blocks[parent2Index];
                parent2Index++;
            }
        }

        return new Wall(childBlocks.ToList());
    }



    private Wall Mutate(Wall wall)
    {
        if (random.NextDouble() < mutationRate)
        {
            int n = wall.Blocks.Count;
            int index1 = random.Next(n);
            int index2 = random.Next(n);

            var mutatedBlocks = new List<Block>(wall.Blocks);
            var temp = mutatedBlocks[index1];
            mutatedBlocks[index1] = mutatedBlocks[index2];
            mutatedBlocks[index2] = temp;

            return new Wall(mutatedBlocks);
        }

        return wall;
    }
}

class Program
{
    static void Main()
    {
        List<Block> blocks = new List<Block>
        {
            new Block(1), new Block(2), new Block(3), new Block(4),
            new Block(5), new Block(6), new Block(7), new Block(8),
            new Block(9), new Block(10)
        };

        GeneticAlgorithm ga = new GeneticAlgorithm(blocks, populationSize: 100, numberOfGenerations: 500, mutationRate: 0.1);
        Wall bestWall = ga.Run();

        Console.WriteLine("Best wall configuration (heights):");
        foreach (var block in bestWall.Blocks)
        {
            Console.Write(block.Height + " ");
        }
        Console.ReadLine();
    }
}

