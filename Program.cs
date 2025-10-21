using System.Linq.Expressions;
using System;

namespace Two_Beasts_in_a_labyrinth
{
    struct Souradnice
    {
        internal int x;
        internal int y;

        public Souradnice(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Prisera
    {
        //Smer prisery je urcen souradnicemi na ktere by se presunula, pokud by sla rovne.
        static Souradnice DOPRAVA = new Souradnice(1, 0);
        static Souradnice NAHORU = new Souradnice(0, -1);
        static Souradnice DOLEVA = new Souradnice(-1, 0);
        static Souradnice DOLU = new Souradnice(0, 1);

        Souradnice[] smery = { DOPRAVA, NAHORU, DOLEVA, DOLU };
        const int pocet_smeru = 4;

        enum smer_otoceni
        {
            DOLEVA,
            DOPRAVA
        }

        enum situace
        {
            //Tento stav charakterizuje dokonceni rozsereni situace NENI_ZED
            DOKONCIT_POHYB,
            //Pokud je po prave ruce zed a muzeme jit vpred, tak to udelame
            ZED_A_VOLNO,
            //Pokud je po prave ruce, ale nemuzeme jit vpred, je potreba se otocit doleva
            ZED_A_NENI_VOLNO,
            //Pokud po prave ruce neni zed, je potreba provest nejprve otoceni doprava a pote krok vpred. 
            NENI_ZED
        }

        internal Souradnice pozice;
        internal int index_smeru;

        bool musi_dokoncit_pohyb = false;
        Bludiste bludiste;

        public Prisera(Bludiste bludiste)
        {
            this.bludiste = bludiste;
        }

        int zjisti_index_noveho_smeru(smer_otoceni smer)
        {
            int novy_index = index_smeru;

            switch (smer)
            {
                case smer_otoceni.DOLEVA:
                    {
                        novy_index = (novy_index + 1) % pocet_smeru;
                        break;
                    }
                case smer_otoceni.DOPRAVA:
                    {
                        novy_index = ((novy_index - 1) + pocet_smeru) % pocet_smeru;
                        break;
                    }
            }

            return novy_index;
        }

        Souradnice krok_danym_smerem(Souradnice smer)
        {
            //vrati souradnice pozice na kterou by se příšera posunula, pokud by udělala jeden krok daným směrem
            int nove_x = pozice.x + smer.x;
            int nove_y = pozice.y + smer.y;
            Souradnice nova_pozice = new Souradnice(nove_x, nove_y);

            return nova_pozice;
        }

        internal void udelej_tah()
        {
            situace zjisti_situaci()
            {
                int index_smeru_doprava = zjisti_index_noveho_smeru(smer_otoceni.DOPRAVA);
                Souradnice po_prave_ruce = krok_danym_smerem(smery[index_smeru_doprava]);
                bool volno_vpravo = bludiste.je_tam_volno(po_prave_ruce);

                Souradnice vpredu = krok_danym_smerem(smery[index_smeru]);
                bool volno_vpredu = bludiste.je_tam_volno(vpredu);

                situace situace;
                if (musi_dokoncit_pohyb)
                {
                    situace = situace.DOKONCIT_POHYB;
                }
                else if (!volno_vpravo && volno_vpredu)
                {
                    situace = situace.ZED_A_VOLNO;
                }
                else if (!volno_vpravo && !volno_vpredu)
                {
                    situace = situace.ZED_A_NENI_VOLNO;
                }
                else //(volno_vpravo)
                {
                    situace = situace.NENI_ZED;
                }

                return situace;
            }

            void proved(situace situace)
            {
                Souradnice nova_pozice = pozice;
                int index_noveho_smeru = index_smeru;

                switch (situace)
                {

                    case situace.DOKONCIT_POHYB:
                        {
                            nova_pozice = krok_danym_smerem(smery[index_smeru]);
                            musi_dokoncit_pohyb = false;
                            break;
                        }
                    case situace.ZED_A_VOLNO:
                        {
                            nova_pozice = krok_danym_smerem(smery[index_smeru]);
                            break;
                        }
                    case situace.ZED_A_NENI_VOLNO:
                        {
                            index_noveho_smeru = zjisti_index_noveho_smeru(smer_otoceni.DOLEVA);
                            break;
                        }
                    case situace.NENI_ZED:
                        {
                            index_noveho_smeru = zjisti_index_noveho_smeru(smer_otoceni.DOPRAVA);
                            musi_dokoncit_pohyb = true;
                            break;
                        }
                }

                pozice = nova_pozice;
                index_smeru = index_noveho_smeru;

                bludiste.posun_priseru(pozice, index_smeru);
            }

            situace situace = zjisti_situaci();
            proved(situace);
        }

    }

    class Bludiste
    {
        //indexy smeru jsou stejne u prisery i u bludiste
        public char[] smery = { '>', '^', '<', 'v' };
        const int pocet_smeru = 4;

        int radku;
        int sloupcu;
        char[,] mapa;
        internal Prisera prisera;
        Souradnice pozice_prisery;

        public Bludiste(int sirka, int vyska)
        {
            radku = vyska;
            sloupcu = sirka;
            mapa = new char[radku, sloupcu];
        }

        internal void nacti_bludiste()
        {
            int vrat_index_smeru(char prisera)
            {
                int index_smeru = 0;

                for (int i = 0; i < pocet_smeru; i++)
                {
                    if (prisera == smery[i])
                    {
                        index_smeru = i;
                        break;
                    }
                }

                return index_smeru;
            }

            char nacti_znak()
            {
                bool hotovo = false;
                char znak = ' ';

                while (!hotovo)
                {
                    znak = Convert.ToChar(Console.Read());
                    if (znak == 'X' || znak == '.' || znak == '<' || znak == '>' || znak == '^' || znak == 'v')
                    {
                        hotovo = true;
                    }
                }

                return znak;
            }

            for (int i = 0; i < radku; i++)
            {
                for (int j = 0; j < sloupcu; j++)
                {
                    char znak = nacti_znak();
                    mapa[i, j] = znak;
                    if (znak != 'X' && znak != '.')
                    {
                        Souradnice pozice = new Souradnice(j, i);
                        int index_smeru = vrat_index_smeru(znak);

                        pozice_prisery = pozice;
                        prisera.pozice = pozice_prisery;
                        prisera.index_smeru = index_smeru;
                    }
                }
            }
        }

        internal bool je_tam_volno(Souradnice pozice)
        {
            char znak = mapa[pozice.y, pozice.x];

            bool vysledek;

            switch (znak)
            {
                case 'X':
                    {
                        vysledek = false;
                        break;
                    }
                default: //'.'
                    {
                        vysledek = true;
                        break;
                    }
            }

            return vysledek;
        }

        internal void posun_priseru(Souradnice pozice, int index_smeru)
        {
            void vymaz_starou()
            {
                mapa[pozice_prisery.y, pozice_prisery.x] = '.';
            }

            void zapis_novou()
            {
                pozice_prisery.x = pozice.x;
                pozice_prisery.y = pozice.y;
                mapa[pozice_prisery.y, pozice_prisery.x] = smery[index_smeru];
            }

            void vykresli_bludiste()
            {
                for (int i = 0; i < radku; i++)
                {
                    for (int j = 0; j < sloupcu; j++)
                    {
                        Console.Write(mapa[i, j]);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
            }

            vymaz_starou();
            zapis_novou();

            vykresli_bludiste();
        }
    }

    internal class Program
    {
        static int nacti_cislo()
        {
            //const int posunuti = 48;

            int cislo = 0;
            bool hotovo = false;
            bool cislo_existuje = false;

            while (hotovo == false)
            {
                int znak = Console.Read();
                //znak = znak - posunuti;

                if (znak <= 9 & znak >= 0)
                {
                    cislo = cislo * 10 + znak;
                    cislo_existuje = true;
                }
                else
                {
                    if (cislo_existuje == true)
                        hotovo = true;
                }
            }
            return cislo;
        }

        static void Main(string[] args)
        {
            int pocet_tahu = 20;

            int sirka = nacti_cislo();
            int vyska = nacti_cislo();

            Bludiste bludiste = new Bludiste(sirka, vyska);
            Prisera prisera = new Prisera(bludiste);
            bludiste.prisera = prisera;

            bludiste.nacti_bludiste();
            //Console.WriteLine(prisera.pozice.x);
            //Console.WriteLine(prisera.pozice.y);
            //Console.WriteLine(prisera.index_smeru);

            for (int i = 0; i < pocet_tahu; i++)
            {
                prisera.udelej_tah();
            }

        }
    }
}



/*
10
6
XXXXXXXXXX
X....X.<.X
X....X...X
X.X..X.X.X
X.X....X.X
XXXXXXXXXX


10
6
XXXXXXXXXX
X....X...X
X....X...X
X.X..X.X.X
X.X.>..X.X
XXXXXXXXXX


9
7
XXXXXXXXX
Xv...X..X
X....X..X
X.X..XX.X
X.X..XX.X
X.X...X.X
XXXXXXXXX
*/
