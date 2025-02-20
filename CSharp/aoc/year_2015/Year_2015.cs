﻿
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using Superpower;
using Superpower.Parsers;
using Superpower.Model;


namespace Year2015;
public static class y_2015
{
    public static void Day1(string Path)
    {
        string usrIn = File.ReadAllText(Path);
        int p1 = 0;
        int p2 = 0;
        bool basement = false;

        for (int i = 0; i < usrIn.Length; i++)
        {
            p1 += usrIn[i] == '(' ? 1 : -1;
            if (!basement && p1 == -1)
            {
                p2 = i + 1;
                basement = !basement;
            }
        }
        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");

    }

    public static void Day2(string Path)
    {
        string[] usrIn = File.ReadLines(Path).Select(l => l).ToArray();
        int p1 = 0;
        int p2 = 0;
        int[] vals = new int[3];
        int[] areas = new int[3];

        foreach (string line in usrIn)
        {
            // l w h
            vals = line.Split('x').Select(c => int.Parse(c)).ToArray();

            areas[0] = vals[0] * vals[1];
            areas[1] = vals[1] * vals[2];
            areas[2] = vals[2] * vals[0];

            p1 += areas.Min();
            p1 += areas.Select(v => v * 2).Sum();

            p2 += (vals.Aggregate((a, b) => a * b));
            p2 += (vals.OrderBy(v => v).Take(2).Select(v => v).Sum() * 2);

        }

        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");
    }

    private static (int, int) GetMovement(char c, int x, int y)
    {
        return c switch
        {
            '>' => (x + 1, y),
            '<' => (x - 1, y),
            '^' => (x, y + 1),
            'v' => (x, y - 1),
            _ => (x, y)
        };
    }

    public static void Day3(string Path)
    {
        string usrIn = File.ReadAllText(Path);

        var (x, y) = (0, 0);
        var (sx, sy) = (0, 0);
        var (rx, ry) = (0, 0);
        var (cx, cy) = (0, 0);
        int p1 = 1;
        int p2 = 1;
        bool santaTurn = false;

        var p1Houses = new Dictionary<(int, int), int> { { (x, y), 1 } };
        var p2Houses = new Dictionary<(int, int), int> { { (x, y), 2 } };

        foreach (char c in usrIn)
        {

            // part 1
            (x, y) = GetMovement(c, x, y);
            if (!p1Houses.ContainsKey((x, y)))
            {
                p1Houses.Add((x, y), 1);
                p1++;
            }

            // part 2
            if (santaTurn)
            {
                (sx, sy) = GetMovement(c, sx, sy);
                (cx, cy) = (sx, sy);
            }
            else
            {
                (rx, ry) = GetMovement(c, rx, ry);
                (cx, cy) = (rx, ry);
            }
            santaTurn = !santaTurn;

            if (!p2Houses.ContainsKey((cx, cy)))
            {
                p2Houses.Add((cx, cy), 1);
                p2++;
            }
        }
        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");
    }

    public static void Day4(string Path)
    {
        string secret = File.ReadAllText(Path).Trim();

        int p1 = 0;
        int p2 = 0;
        var hasher = MD5.Create();
        int i = 0;
        byte[] toHash = new byte[20];
        string hashed;

        while (p1 == 0 || p2 == 0)
        {
            toHash = $"{secret}{i}".Select(c => (byte)c).ToArray();
            hashed = BitConverter.ToString(hasher.ComputeHash(toHash)).Replace("-", "");
            if (p1 == 0 && hashed.Substring(0, 5) == "00000")
            {
                Console.WriteLine($"Found p1: {hashed}");
                p1 = i;
            }
            if (p2 == 0 && hashed.Substring(0, 6) == "000000")
            {
                Console.WriteLine($"Found p2: {hashed}");
                p2 = i;
            }
            i++;
        }
        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");
    }

    [Flags]
    private enum Niceness { Def = 0, Vowel = 2, Double = 4, Bad = 8, Repeats = 16, Pair = 32 };
    private static bool d5P1(string line)
    {
        var badStrings = new List<string> { "ab", "cd", "pq", "xy" };
        var allVowels = new List<char> { 'a', 'e', 'i', 'o', 'u' };
        Niceness flags = default;
        int vowels = 0;

        for (int i = 0; i < line.Length; i++)
        {

            char c = line[i];

            if (allVowels.Contains(c)) vowels++;

            if (i < 1) continue;
            char lc = line[i - 1];
            if (badStrings.Contains($"{lc}{c}"))
            {
                flags |= Niceness.Bad;
                break;
            }
            if (lc == c) flags |= Niceness.Double;
        }
        if (flags.HasFlag(Niceness.Bad)) return false;

        if (vowels > 2) flags |= Niceness.Vowel;

        return flags.HasFlag(Niceness.Vowel | Niceness.Double);
    }

    private static bool d5P2(string line)
    {
        Niceness flags = default;
        for (int i = 0; i < line.Length; i++)
        {
            if (i < line.Length - 2)
            {
                string asdf = line[i..(i + 2)];
                if (line[(i + 2)..].Contains(asdf)) flags |= Niceness.Pair;
            }

            if (i < 2) continue;
            char llc = line[i - 2];
            char lc = line[i - 1];
            char c = line[i];

            if (llc == c) flags |= Niceness.Repeats;
        }

        return flags.HasFlag(Niceness.Repeats | Niceness.Pair);
    }

    public static void Day5(string Path)
    {
        string[] usrIn = File.ReadAllLines(Path).ToArray();
        int p1 = 0;
        int p2 = 0;

        foreach (string line in usrIn)
        {
            p1 += d5P1(line) == true ? 1 : 0;
            p2 += d5P2(line) == true ? 1 : 0;
        }
        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");

    }
    private enum States { Off, On, Toggle };
    public static void Day6(string Path)
    {
        string[] usrIn = File.ReadAllLines(Path).Select(l => l).ToArray();
        var gridP1 = new int[1000, 1000];
        var gridP2 = new int[1000, 1000];
        int p1 = 0;
        int p2 = 0;


        TextParser<(States instruction, (int x1, int y1, int x2, int y2) coords)> parser =
            (from instructs in Character.Letter.Or(Character.WhiteSpace).AtLeastOnce()
             from coordFromX in Character.Digit.AtLeastOnce()
             from _ in Character.EqualTo(',')
             from coordFromY in Character.Digit.AtLeastOnce()
             from __ in Span.EqualTo(" through ")
             from coordToX in Character.Digit.AtLeastOnce()
             from ___ in Character.EqualTo(',')
             from coordToY in Character.Digit.AtLeastOnce()
             select (new string(instructs) switch { "turn on " => States.On, "turn off " => States.Off, _ => States.Toggle },
             (int.Parse(coordFromX), int.Parse(coordFromY), int.Parse(coordToX), int.Parse(coordToY)))
            );



        foreach (string line in usrIn)
        {
            var currInst = parser.Parse(line);

            switch (currInst.instruction)
            {
                case States.On:
                    RegionSetter(currInst.coords, 1);
                    break;
                case States.Off:
                    RegionSetter(currInst.coords, 0);
                    break;
                default:
                    RegionToggle(currInst.coords);
                    break;
            }
        }

        void RegionToggle((int x1, int y1, int x2, int y2) coords)
        {
            for (int i = coords.x1; i <= coords.x2; i++)
            {
                for (int j = coords.y1; j <= coords.y2; j++)
                {
                    gridP1[i, j] = gridP1[i, j] == 0 ? 1 : 0;
                    gridP2[i, j] += 2;
                }
            }
        }

        void RegionSetter((int x1, int y1, int x2, int y2) coords, int state)
        {
            for (int i = coords.x1; i <= coords.x2; i++)
            {
                for (int j = coords.y1; j <= coords.y2; j++)
                {
                    gridP1[i, j] = state;
                    gridP2[i, j] += state == 0 ? -1 : 1;
                }
            }
        }


        p1 = gridP1.Cast<int>().Sum();
        p2 = gridP2.Cast<int>().Sum();

        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");

    }


    private enum Ops { And, Or, Lshift, Rshift };


    private static void performOp(Ops op, int val, int val2 = 0)
    {
    }
    public static void Day7(string Path)
    {
        string[] usrIn = File.ReadAllLines(Path).ToArray();

    }



    private static int d8P1(string line)
    {
        int stringLit = line.Length;
        int strSize = 0;
        int i = 1;
        char c = line[1];

        while (c != '"')
        {
            switch (c)
            {
                case '\\':
                    if (line[i + 1] == 'x') i += 4;
                    else i += 2;
                    strSize++;
                    break;

                default:
                    i++;
                    strSize++;
                    break;
            }
            c = line[i];
        }
        return stringLit - strSize;
    }

    private static int d8P2(string line)
    {
        int stringLit = line.Length;
        string newString = "\"";

        foreach (char c in line)
        {
            switch (c)
            {
                case '"':
                    newString += "\\\"";
                    break;

                case '\\':
                    newString += "\\\\";
                    break;

                default:
                    newString += c;
                    break;
            }
        }
        newString += "\"";
        return newString.Length - stringLit;
    }

    public static void Day8(string Path)
    {
        string[] usrIn = File.ReadAllLines(Path).ToArray();


        int p1 = 0;
        int p2 = 0;

        foreach (string line in usrIn)
        {
            p1 += d8P1(line);
            p2 += d8P2(line);
        }
        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");
    }

    public static void Day9(string Path)
    {
        string[] usrIn = File.ReadAllLines(Path).ToArray();
        int p1 = 0;
        int p2 = 0;

        List<City> citiesList = new();

        TextParser<(string inCity, string outCity, int dist)> parser =
            (from City1 in Character.Letter.AtLeastOnce()
             from _ in Span.EqualTo(" to ")
             from City2 in Character.Letter.AtLeastOnce()
             from __ in Span.EqualTo(" = ")
             from distance in Character.Digit.AtLeastOnce()
             select (new string(City1), new String(City2), int.Parse(distance))
            );

        foreach (string line in usrIn)
        {
            var (incity, outcity, dist) = parser.Parse(line);
			//if (rootCity == null) rootCity = new City(incity);
			var curCity = citiesList.FirstOrDefault(c => c.name == incity);

			if(curCity == null)
			{
				curCity = new City(incity);
				citiesList.Add(curCity);
			}
			var newCity = citiesList.FirstOrDefault(c => c.name == outcity);
			if (newCity == null)
			{
				newCity = new City(outcity);
				citiesList.Add(newCity);
			}
			curCity.addCity(newCity, dist);

            //citiesList.Add(rootCity.addCity(incity, new City(outcity), dist)!);
        }



		(p1, p2) = City.FindShortest(citiesList);


        Console.WriteLine($"Part 1: {p1}\nPart 2: {p2}");
    }
}

class City : IEquatable<City>
{
    public string name;
    public record Connection(City City, int Distance);
    public List<Connection> connections;


    public City(string name)
    {
        this.name = name;
        connections = new();
    }

    public City addCity(City inCity, int distance)
    {
        connections.Add(new(inCity, distance));
        inCity.connections.Add(new(this, distance));
        return inCity; //and that
    }

	public Connection GetConnection(City city)
	{
		return connections.First(c => c.City == city);
	}

	public static (int min, int max) FindShortest(List<City> cities)
	{
		var min = int.MaxValue;
		var max = int.MinValue;
		var result = GeneratePermutations(cities);

		for (int i = 0; i < result.Count; i++)
		{
			var route = result[i];
			var cost = 0;
			for (int j = 0; j < route.Count - 1; j++)
			{
				var city = route[j];
				var next = route[j + 1];
				cost += city.GetConnection(next).Distance;
			}
			if (min > cost)
				min = cost;
			if(max < cost)
				max = cost;
		}
		return (min, max);

	}


	private static List<List<T>> GeneratePermutations<T>(List<T> cities)
	{
		return permutate(cities, Enumerable.Empty<T>());
		List<List<T>> permutate(IEnumerable<T> reminder, IEnumerable<T> prefix)
		{
			return !reminder.Any() 
				? new List<List<T>> { prefix.ToList() } 
				: reminder.SelectMany((c, i) => permutate(
					reminder.Take(i).Concat(reminder.Skip(i + 1)).ToList(),
					prefix.Append(c))).ToList();
		}
	}

    public bool Equals(City? other)
    {
        if (other == null) return false;
        return name == other.name;
    }

    public override string ToString()
    {
        return name;
    }
}
