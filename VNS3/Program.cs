using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO;

public class Box
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Depth { get; set; }
    public int Height { get; set; }
    public int boxCount { get; set; }
    public int Volume => Width * Depth * Height;
}

public class Container
{
    public int Width { get; set; } = 587;
    public int Depth { get; set; } = 233;
    public int Height { get; set; } = 220;
    public List<(Box box, int x, int y, int z)> PlacedBoxes { get; set; } = new List<(Box box, int x, int y, int z)>();

    public bool CanPlaceBox(Box box, int x, int y, int z)
    {
        // Kutunun konteyner sınırları içerisinde olup olmadığını kontrol et
        if (x + box.Width > Width || y + box.Depth > Depth || z + box.Height > Height)
            return false;

        // Kutunun başka bir kutu ile çakışıp çakışmadığını kontrol et
        foreach (var (placedBox, px, py, pz) in PlacedBoxes)
        {
            if (x < px + placedBox.Width && x + box.Width > px &&
                y < py + placedBox.Depth && y + box.Depth > py &&
                z < pz + placedBox.Height && z + box.Height > pz)
            {
                return false;
            }
        }

        // Kutunun altının en az yarısının dolu olup olmadığını kontrol et
        if (z > 0 && !IsSupported(box, x, y, z))
            return false;

        return true;
    }

    public bool IsSupported(Box box, int x, int y, int z)
    {
        long supportedArea = 0;
        long boxArea = (long)box.Width * box.Depth;

        foreach (var (placedBox, px, py, pz) in PlacedBoxes)
        {
            if (pz + placedBox.Height == z)
            {
                int overlapWidth = Math.Min(x + box.Width, px + placedBox.Width) - Math.Max(x, px);
                int overlapDepth = Math.Min(y + box.Depth, py + placedBox.Depth) - Math.Max(y, py);

                if (overlapWidth > 0 && overlapDepth > 0)
                {
                    supportedArea += (long)overlapWidth * overlapDepth;
                }
            }
        }

        return supportedArea >= (boxArea / 2);
    }

    public void PlaceBox(Box box, int x, int y, int z)
    {
        PlacedBoxes.Add((box, x, y, z));
    }

    public List<(int x, int y, int z)> GetPotentialPositions()
    {
        var positions = new List<(int x, int y, int z)>();

        // Konteynerin başlangıç pozisyonunu ekle
        positions.Add((0, 0, 0));

        // Yerleştirilmiş kutuların köşelerine göre potansiyel pozisyonları ekle
        foreach (var (box, x, y, z) in PlacedBoxes)
        {
            positions.Add((x + box.Width, y, z));
            positions.Add((x, y + box.Depth, z));
            positions.Add((x, y, z + box.Height));
            positions.Add((x + box.Width, y + box.Depth, z));
            positions.Add((x + box.Width, y, z + box.Height));
            positions.Add((x, y + box.Depth, z + box.Height));
            positions.Add((x + box.Width, y + box.Depth, z + box.Height));
        }

        // x, y, z önceliklendirmesine göre sırala
        return positions.OrderBy(p => p.x).ThenBy(p => p.y).ThenBy(p => p.z).ToList();
    }

    public long CalculateTotalVolume()
    {
        return PlacedBoxes.Sum(pb => (long)pb.box.Width * pb.box.Depth * pb.box.Height);
    }
}

public class Program
{

    static Random random = new Random();
    public static List<double> volumesOfBoxes = new List<double>();
    public static void Main()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Console.WriteLine("Select a Dateset 1-7");
        int datasetNumer = Convert.ToInt32(Console.ReadLine());
        var boxes = GetBoxes(datasetNumer);
        // her kutu grubunun o an ki indexi
        var groupIndex = 0;
        // her grupta ki item sayısı
        var boxCountPerGroup = GetBoxCountPerGroup(datasetNumer);
        // toplam kutu sayısı
        var boxCount = boxes.Count();

        while ((groupIndex * boxCountPerGroup) < boxCount)
        {
            List<Box> group = new List<Box>();
            var currentIndex = groupIndex * boxCountPerGroup;
            for (var i = 0; i < boxCountPerGroup; i++)
            {
                for (int ibox = 0; ibox < boxes[currentIndex + i].boxCount; ibox++)
                {
                    group.Add(boxes[currentIndex + i]);
                }
            }
            groupIndex++;
            OperateBoxes(group);
        }
        PrintVolumes(volumesOfBoxes);
        stopwatch.Stop();
        Console.WriteLine("time is: " + stopwatch.Elapsed);
    }
    public static int GetBoxCountPerGroup(int datasetNumber)
    {
        var list = new List<int> { 3, 5, 8, 10, 12, 15, 20};
        return list[datasetNumber - 1];
    }
    static int ccc = 1;
    public static List<double> initialSolutions = new List<double>();
    public static void OperateBoxes(List<Box> boxes)
    {
        var container = new Container();

        boxes = boxes.OrderByDescending(b => b.Volume).ToList(); // Kutuları büyükten küçüğe sırala

        // Başlangıç çözümünü oluştur
        var unplacedBoxes = new List<Box>();
        foreach (var box in boxes)
        {
            if (!PlaceBoxInContainer(container, box))
            {
                unplacedBoxes.Add(box);
            }
        }

        long bestVolume = container.CalculateTotalVolume();
        int noImprovementCount = 0;
        long containerVolume = 30089620;
        double sonuc2 = Convert.ToDouble((double)bestVolume / containerVolume);

        initialSolutions.Add(sonuc2);
        Console.WriteLine($"{ccc} - initial solution: {sonuc2}");

        // Variable Neighborhood Search algoritması
        double volumeValue = 0;
        while (noImprovementCount < 1)
        {
            int divider = 2;
            if (noImprovementCount % 100 == 0 && noImprovementCount != 0) { Console.WriteLine(noImprovementCount.ToString()); }
            var newContainer = new Container();

            // Konteynerin sadece bir kısmını boşalt ve yeniden yerleştir
            int removeCount = random.Next(1, container.PlacedBoxes.Count / divider); //kutu seç
            //int removeCount = container.PlacedBoxes.Count() / divider;
            var boxesToReposition = container.PlacedBoxes.OrderBy(x => random.Next()).Take(removeCount).ToList();

            // Geri kalan kutuları tut
            var remainingBoxes = container.PlacedBoxes.Except(boxesToReposition).ToList();

            // Çıkarılan ve hiç yerleştirilememiş kutuları birleştir
            var allBoxesToPlace = boxesToReposition.Select(pb => pb.box).Concat(unplacedBoxes).ToList();

            // Yeni konteynere geri kalan kutuları yerleştir
            foreach (var (box, x, y, z) in remainingBoxes)
            {
                newContainer.PlaceBox(box, x, y, z);
            }

            // Birleştirilen kutuları rastgele sırayla yeniden yerleştir
            allBoxesToPlace = allBoxesToPlace.OrderBy(x => random.Next()).ToList();
            foreach (var box in allBoxesToPlace)
            {
                PlaceBoxInContainer(newContainer, box);
            }

            long newVolume = newContainer.CalculateTotalVolume();
            if (newVolume > bestVolume)
            {
                container = newContainer;
                bestVolume = newVolume;
                noImprovementCount = 0;
                volumeValue = Convert.ToDouble((double)bestVolume / containerVolume);

                double sonuc = Convert.ToDouble((double)bestVolume / containerVolume);
                Console.WriteLine($"{ccc} - New best volume: {sonuc}");
                divider++;
            }
            else
            {
                noImprovementCount++;
                volumeValue = Convert.ToDouble((double)bestVolume / containerVolume);

            }
            //if (noImprovementCount == 9 && newVolume < bestVolume)
            //{
            //    volumesOfBoxes.Add((double)bestVolume/containerVolume);
            //}
        }
        volumesOfBoxes.Add(Convert.ToDouble(volumeValue));
        PrintContainer(container);
        ccc++;

    }
    public static List<Box> GetBoxes(int datasetNumber)
    {
        List<Box> boxes = new List<Box>();
        using (StreamReader sr = new StreamReader($"br{datasetNumber.ToString()}.txt"))
        {
            string satir;
            int kutuIndex = 0;
            while ((satir = sr.ReadLine()) != null)
            {
                string[] sira = satir.Split(' ');
                var depth = Convert.ToInt32(sira[2]);
                var width = Convert.ToInt32(sira[4]);
                var height = Convert.ToInt32(sira[6]);
                var boxCount = Convert.ToInt32(sira[8]);
                boxes.Add(new Box { Name = "kutu" + kutuIndex.ToString(), Depth = depth, Width = width, Height = height, boxCount = boxCount });
                kutuIndex++;
            }
        }

        return boxes;
    }
    public static bool PlaceBoxInContainer(Container container, Box box)
    {
        var positions = container.GetPotentialPositions();

        foreach (var (x, y, z) in positions)
        {
            if (container.CanPlaceBox(box, x, y, z))
            {
                container.PlaceBox(box, x, y, z);
                return true;
            }
        }
        return false;
    }
    static int boxesNumber = 1;
    public static void PrintContainer(Container container)
    {
        //foreach (var (box, x, y, z) in container.PlacedBoxes)
        //{
        //    Console.WriteLine($"Box {box.Name} placed at ({x}, {y}, {z})");
        //}

        Console.WriteLine(container.PlacedBoxes.Count());

        string txtName = "boxes"+boxesNumber.ToString()+".txt";
        //File.Delete("boxes.txt");
        StreamWriter yaz = new StreamWriter(txtName, true);
        foreach (var item in container.PlacedBoxes)
        {
            yaz.WriteLine($"(\"{item.box.Name}\",  ({item.x},{item.y},{item.z}), {item.box.Width},{item.box.Depth},{item.box.Height}),");
        }
        yaz.Close();
        boxesNumber++;
    }
    public static void PrintVolumes(List<Double> volumes)
    {
        Console.WriteLine("volume list");
        volumes.Sort();
        int index = 1;
        double avarage = 0;
        foreach (var volume in volumes)
        {
            Console.WriteLine($"{index} - {volume}");
            avarage += volume;
            index++;
        }
        Console.WriteLine("Avarage is: " + avarage / volumes.Count());
        Console.WriteLine("Lowest is: " + volumes[0]);
        Console.WriteLine("Biggest is: " + volumes[volumes.Count() - 1]);
        double initialAvarage = 0;
        foreach (var item in initialSolutions)
        {
            initialAvarage = initialAvarage + item;
        }
        Console.WriteLine("initial solutions avarage is: "+initialAvarage/initialSolutions.Count());


    }
}