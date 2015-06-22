using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MapInterpritator
{
    public MapInterpritator()
    {
    }

    public CellType[,] LoadMapFromFile(string path)
    {
        var lines = File.ReadAllLines(path).ToList();
        CellType[,] matrix = null;
        lines.Reverse();
        int i = 0;
        foreach (var line in lines)
        {
            int j = 0;
            foreach (var ch in line)
            {
                if (matrix == null) matrix = new CellType[line.Length, lines.Count];
                matrix[j, i] = (CellType)Enum.Parse(typeof(CellType), ch.ToString());
                j++;
            }
            i++;
        }
        return matrix;
    }


    public ContentType[,] LoadContentFromFile(string path)
    {
        var lines = File.ReadAllLines(path).ToList();
        return LoadContentFromStringList(lines);
    }

    public ContentType[,] LoadContentFromStringList(List<string> lines)
    {
        lines.Reverse();
        ContentType[,] content = new ContentType[lines[0].Length, lines.Count];
        int i = 0;
        foreach (var line in lines)
        {
            int j = 0;
            foreach (var ch in line)
            {
                content[j, i] = (ContentType)Enum.Parse(typeof(ContentType), ch.ToString());
                j++;
            }
            i++;
        }
        return content;
    }
}
