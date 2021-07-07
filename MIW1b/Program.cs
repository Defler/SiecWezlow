using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MIW1b
{
    public class Wezel
    {
        public int id;
        List<Krawedz> listaKrawedzi;

        public Wezel(int id)
        {
            this.id = id;
            this.listaKrawedzi = new List<Krawedz>();
        }

        public void DodajKrawedz(Krawedz krawedz)
        {
            this.listaKrawedzi.Add(krawedz);
        }
    }

    public class Krawedz
    {
        public int id;
        public double wartosc;
        Wezel wezelPierwszy;
        Wezel wezelDrugi;

        public Krawedz(int id, double wartosc, Wezel wezelPierwszy, Wezel wezelDrugi)
        {
            this.id = id;
            this.wartosc = wartosc;
            this.wezelPierwszy = wezelPierwszy;
            this.wezelDrugi = wezelDrugi;
        }

        public Krawedz(int id, double wartosc, Wezel wezelDrugi)
        {
            this.id = id;
            this.wartosc = wartosc;
            this.wezelDrugi = wezelDrugi;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            //Nie trzeba ustawiać nowego seeda w randomie, bo caly czas pracuje na jednym obiekcie random, wiec za kazdym razem bede mial inny wynik

            double zakresMin = 0;
            double zakresMax = 100;



            int[] tablicaKolumnWezlow;  //tablica wielkoscia rowna liczbie kolumn wezlow, przechowuje ilosc wezlow w kolejnych kolumnach
            List<Wezel> listaWezlow = new List<Wezel>();
            Wezel[][] tablicaWezlow;  //tablica tablic zgodna ze schematem ukladu (np. dla 3-2-2, tablica ma 3 tablice z wielkosciami kolejno 3,2,2)
            bool czyWczytywanie = false;  
            double[][] daneWczytane = null;  //tablica tablic przechowuja wartosci wczytane jako int
            string[] daneWczytaneS;  //tablica przechowujaca wczytane dane jako string
            string linia;  //string przechowujacy schemat (np. "3-2-2")

            Console.WriteLine("Czy chcesz wczytac dane z pliku? [y - tak]");
            if(Console.ReadKey().KeyChar == 'y')
            {
                int indeksSegmentu = 0;
                czyWczytywanie = true;
                daneWczytaneS = File.ReadAllLines("zad1b.txt");
                linia = File.ReadLines("zad1b.txt").First();  //wczytanie schematu
                daneWczytane = new double[daneWczytaneS.Length-1][]; //tablica int z wartosciami krawedzi (length -1 bo pierwsza linia to schemat)
                foreach(string line in daneWczytaneS)
                {
                    if (line == linia)
                        continue; //pierwsza linia jest pomijana przy odczycie wartosci z pliku
                    string[] liniaS = line.Split(' ');
                    double[] nowaLiniaDanych = new double[liniaS.Length];
                    for (int i=0; i<liniaS.Length; i++)
                    {
                        nowaLiniaDanych[i] = Double.Parse(liniaS[i]);
                    }
                    daneWczytane[indeksSegmentu] = nowaLiniaDanych;
                    indeksSegmentu++;
                }
            }
            else
            {
                Console.WriteLine("\nPodaj schemat struktury, separator to myslnik: ");
                linia = Console.ReadLine();

                //odczytywanie dolnej i gornej wartosci krawedzi

                Console.WriteLine("Podaj dolny zakres wartosci krawedzi: ");
                zakresMin = Double.Parse(Console.ReadLine());

                Console.WriteLine("Podaj gorny zakres wartosci krawedzi: ");
                zakresMax = Double.Parse(Console.ReadLine());

                Console.WriteLine($"Wartosc min: {zakresMin}   Zakres max: {zakresMax}\n");
            }

            string[] schemat = linia.Split('-');
            tablicaKolumnWezlow = new int[schemat.Length];


            Console.WriteLine($"\n\nLiczba kolumn wezlow: {schemat.Length}, ilosc wezlow w kolumnach: ");
            for(int i=0; i<schemat.Length; i++)
            {
                tablicaKolumnWezlow[i] = Int32.Parse(schemat[i]);
                Console.WriteLine($"kol{i} - {tablicaKolumnWezlow[i]}");
            }

            //tworzenie wezlow
            int idWezla = 0;
            for(int i=0; i< tablicaKolumnWezlow.Length; i++)
            {
                for(int j=0; j< tablicaKolumnWezlow[i]; j++)
                {
                    Wezel nowyWezel = new Wezel(idWezla);
                    listaWezlow.Add(nowyWezel);
                    idWezla++;
                }
            }

            //wpisywanie id do tablicy tablic stworzonej w schemacie podanym przez gracza
            idWezla = 0;
            tablicaWezlow = new Wezel[tablicaKolumnWezlow.Length][];
            for(int i=0; i< tablicaKolumnWezlow.Length; i++)
            {
                Wezel[] nowySegment = new Wezel[tablicaKolumnWezlow[i]];
                for(int j=0; j<nowySegment.Length; j++)
                {
                    nowySegment[j] = listaWezlow[idWezla];
                    idWezla++;
                }
                tablicaWezlow[i] = nowySegment;
            }

            //wypisywanie kolejnych id wezlow zgodnie ze schematem (wiersz to kolumna)
            Console.WriteLine();
            for(int i=0; i<tablicaWezlow.Length; i++)
            {
                for (int j = 0; j < tablicaWezlow[i].Length; j++)
                    Console.Write($"{tablicaWezlow[i][j].id} ");
                Console.WriteLine();
            }


            //tworzenie krawedzi i wpisywanie ich w odpowiednie miejsce w tablicy
            int idKrawedzi = 0;
            int licznikKrawedziWSegmencie;
            Krawedz[][] tablicaKrawedzi = new Krawedz[tablicaWezlow.Length - 1][];
            for(int i=0; i<tablicaWezlow.Length-1; i++)
            {
                licznikKrawedziWSegmencie = 0;
                Krawedz[] nowySegmentK = new Krawedz[tablicaWezlow[i].Length * tablicaWezlow[i + 1].Length + tablicaWezlow[i+1].Length];
                //for w forze, ktory dodaje krawedzie laczace pierwsza i druga kolumne wezlow
                for(int j=0; j<tablicaWezlow[i].Length; j++)
                {
                    for(int k=0; k<tablicaWezlow[i+1].Length; k++)
                    {
                        //rnd = new Random(rnd.Next());
                        Krawedz nowaKrawedz = new Krawedz(idKrawedzi, rnd.NextDouble() * (zakresMax - zakresMin) + zakresMin, tablicaWezlow[i][j], tablicaWezlow[i + 1][k]);
                        if (czyWczytywanie)
                            nowaKrawedz.wartosc = daneWczytane[i][licznikKrawedziWSegmencie]; //jezeli jest wczytywanie z pliku to wartosc jest nadpisywana wartoscia z pliku

                        idKrawedzi++;
                        nowySegmentK[licznikKrawedziWSegmencie] = nowaKrawedz;
                        licznikKrawedziWSegmencie++;
                    }
                }

                //drugi for dodawajacy 'ogonki' do segmentu
                for(int j=0; j < tablicaWezlow[i + 1].Length; j++)
                {
                    //rnd = new Random(rnd.Next());
                    Krawedz nowaKrawedz = new Krawedz(idKrawedzi, rnd.NextDouble() * (zakresMax - zakresMin) + zakresMin, tablicaWezlow[i + 1][j]);
                    if (czyWczytywanie)
                        nowaKrawedz.wartosc = daneWczytane[i][licznikKrawedziWSegmencie]; //jezeli jest wczytywanie z pliku to wartosc jest nadpisywana wartoscia z pliku
                    idKrawedzi++;
                    nowySegmentK[licznikKrawedziWSegmencie] = nowaKrawedz;
                    licznikKrawedziWSegmencie++;
                    //Console.Write($"{nowaKrawedz.wartosc} ");
                }
                tablicaKrawedzi[i] = nowySegmentK;
            }

            //wyswietlanie kolejnych krawedzi wedlug schematu
            Console.WriteLine();
            Console.WriteLine("Lista krawedzi w segmentach: ");
            for(int i=0; i<tablicaKrawedzi.Length; i++)
            {
                for (int j = 0; j < tablicaKrawedzi[i].Length; j++)
                    Console.Write($"{tablicaKrawedzi[i][j].wartosc.ToString("F2")} ");
                Console.WriteLine();
            }

            //zapis do pliku
            //pierwszy wiersz to schemat, kolejne wiersze to segmenty z kolejnymi wartosciami krawedzi
            Console.WriteLine("\nCzy zapisac dane? [y - tak]");
            if (Console.ReadKey().KeyChar == 'y')
            {
                using StreamWriter file = new StreamWriter("zad1b.txt");
                file.WriteLine(linia);
                string nowaLinia;
                for (int i = 0; i < tablicaKrawedzi.Length; i++)
                {
                    nowaLinia = "";
                    for (int j = 0; j < tablicaKrawedzi[i].Length; j++)
                        nowaLinia += $"{tablicaKrawedzi[i][j].wartosc} ";
                    nowaLinia = nowaLinia.Remove(nowaLinia.Length - 1); //usuwanie zbednych spacji na koncu wiersza
                    file.WriteLine(nowaLinia);
                }
            }

            Console.WriteLine("\n\nKoniec przedstawienia.");
            
        }
    }
}
