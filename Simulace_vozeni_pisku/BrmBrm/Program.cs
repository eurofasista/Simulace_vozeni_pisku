using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

class Program
{
    static void Main(string[] args)
    {
        string soubor = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent + "\\data\\simulace_4.txt";
        StreamReader sr = new StreamReader(soubor);
        List<Auto> auta = new List<Auto>();
        int hromada = Int32.Parse(sr.ReadLine());
        while ((soubor = sr.ReadLine()) != "---")
        {
            string[] s = soubor.Split(' ');
            auta.Add(new Auto(s[0], Int32.Parse(s[1]), Int32.Parse(s[2]), Int32.Parse(s[3]), Int32.Parse(s[4])));
        }
        int fofr = 0;
        int[] nejlepsi = new int[4];
        for(int a = -5; a <= 5; a++) for (int b = -5; b <= 5; b++) for (int c = -5; c <= 5; c++) for (int d = -5; d <= 5; d++)
                    {
                        int cas = Simulace(auta, hromada, new int[4] {a, b, c, d}, true);
                        if(fofr == 0 || fofr > cas) { nejlepsi = new int[4] { a, b, c, d }; fofr = cas; }
                    }
        Console.WriteLine(nejlepsi[0] + "  " + nejlepsi[1] + "  " + nejlepsi[2] + "  " + nejlepsi[3]);
        Console.WriteLine(fofr);
        Console.ReadKey();
    }

    static int Simulace(List<Auto> auta, int hromada, int[] kriteria, bool chytra_simulace)
    {
        int cas = 0;
        foreach (Auto a in auta) a.Reset();
        while(hromada > 0)
        {
            IEnumerable<Auto> l = auta.Where(x => x.KDizpozici <= cas);
            if (l.Count() > 0)
            {
                Auto a = null;
                if (chytra_simulace) a = VyberAleChytre(l, kriteria);
                else a = VyberHloupe(l);
                //Console.WriteLine(a.Jmeno + ": nakládka v čase " + cas);
                hromada -= a.Nosnost;
                a.KDizpozici = cas + a.DobaNakladani + a.DobaVykladani + 2 * a.DobaCesty;
                cas += a.DobaNakladani;
            }
            else
            {
                int dalsi_cas = auta.ElementAt(0).KDizpozici;
                foreach (Auto autó in auta) if (autó.KDizpozici < dalsi_cas) dalsi_cas = autó.KDizpozici;
                cas = dalsi_cas;
            }
        }
        int celkovy_cas = auta.ElementAt(0).KDizpozici;
        foreach (Auto autó in auta) if (autó.KDizpozici > celkovy_cas) celkovy_cas = autó.KDizpozici;
        //Console.WriteLine(celkovy_cas);
        return celkovy_cas;
    }

    static Auto VyberAleChytre(IEnumerable<Auto> auta, int[] kriteria)
    {
        List<Auto> kandidati = auta.ToList();
        int kriterium = Kriterium(kandidati, kriteria[0], kriteria[1], kriteria[2], kriteria[3]);
        kandidati = kandidati.Where(a => a.Nosnost * kriteria[0] + a.DobaNakladani * kriteria[1] + a.DobaVykladani * kriteria[2] + a.DobaCesty * kriteria[3] == kriterium).ToList();
        return kandidati.ElementAt(0);
    }

    static Auto VyberHloupe(IEnumerable<Auto> auta)
    {
        List<Auto> kandidati = auta.ToList();
        int[,] kriteria = new int[4, 4];
        for (int i = 0; i < 16; i++) kriteria[i / 4, i % 4] = 0;
        kriteria[0, 0] = 1;
        kriteria[1, 3] = -1;
        kriteria[2, 1] = -1;
        kriteria[3, 2] = -1;
        for(int i = 0; i < 4; i++)
        {
            int kriterium = Kriterium(kandidati, kriteria[i, 0], kriteria[i, 1], kriteria[i, 2], kriteria[i, 3]);
            kandidati = kandidati.Where(a => a.Nosnost * kriteria[i, 0] + a.DobaNakladani * kriteria[i, 1] + a.DobaVykladani * kriteria[i, 2] + a.DobaCesty * kriteria[i, 3] == kriterium).ToList();
        }
        return kandidati.ElementAt(0);
    }

    static int Kriterium(IEnumerable<Auto> auta, int nosnost, int nakl, int vykl, int cesta)
    {
        Auto aa = auta.ElementAt(0);
        int krit = aa.Nosnost * nosnost + aa.DobaNakladani * nakl + aa.DobaVykladani * vykl + aa.DobaCesty * cesta;
        foreach (Auto a in auta)
        {
            if (a.Nosnost * nosnost + a.DobaNakladani * nakl + a.DobaVykladani * vykl + a.DobaCesty * cesta > krit) krit = a.Nosnost * nosnost + a.DobaNakladani * nakl + a.DobaVykladani * vykl + a.DobaCesty * cesta;
        }
        return krit;
    }
}


public class Auto
{
    public string Jmeno;
    public int Nosnost;
    public int DobaCesty;
    public int DobaNakladani;
    public int DobaVykladani;
    public int KDizpozici;
    public Auto(string jmeno, int nosnost, int doba_cesty, int doba_nakladani, int doba_vykladani)
    {
        Jmeno = jmeno;
        KDizpozici = 0;
        Nosnost = nosnost;
        DobaNakladani = doba_nakladani;
        DobaVykladani = doba_vykladani;
        DobaCesty = doba_cesty;
    }
    public void Reset()
    {
        KDizpozici = 0;
    }
}