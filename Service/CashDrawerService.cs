using System.Text;
using PFG_BackEnd.Helper;
using PFG_BackEnd.ModelsDTO;

namespace PFG_BackEnd.Service
{
    public class CashDrawerService
    {
        private const string PrinterName = "Impresora Tickets";
        private const string KitchenPrinterName = "Impresora Tickets"; // canviar nom impressora quan estigui

        private const string EmpresaDni = "DNI: 33954762T";
        private const string EmpresaDireccio = "Avinguda d'Ildefons Cerdà, 82\n08540 Centelles";
        
        private const int LineWidth = 30; // guions "------------------------------" 

        public void OpenDrawer()
        {
            byte[] openDrawer = { 0x1B, 0x70, 0x00, 0x32, 0x32 };
            RawPrinterHelper.SendBytes(PrinterName, openDrawer);
        }

        public void PrintTicketFinalAsync(IEnumerable<ImprimirTicketDTO> linies)
        {
            var encoding = Encoding.GetEncoding(1252);
            var bytes = new List<byte>();

            AddBytes(bytes, 0x1B, 0x40);         //init     

            AddBytes(bytes, 0x1B, 0x61, 0x01);
            AddText(bytes, encoding, "TICKET VENTA\n");

            AddText(bytes, encoding, EmpresaDni + "\n");
            AddText(bytes, encoding, EmpresaDireccio + "\n");

            AddText(bytes, encoding, new string('-', LineWidth) + "\n");

            AddBytes(bytes, 0x1B, 0x61, 0x00);      // esquerra
            AddText(bytes, encoding, $"Data: {DateTime.Now:dd/MM/yyyy HH:mm}\n");
            AddText(bytes, encoding, new string('-', LineWidth) + "\n");
            AddText(bytes, encoding, "Producte       Quantitat   Preu\n");
            AddText(bytes, encoding, new string('-', LineWidth) + "\n");

            double total = 0;

            foreach (var l in linies)
            {
                total += l.TotalLinia;
                AddText(bytes, encoding, FormatLinia(l.NomProducte, l.Quantitat, l.TotalLinia));
            }

            AddText(bytes, encoding, new string('-', LineWidth) + "\n");

            AddBytes(bytes, 0x1B, 0x61, 0x01);       
            AddText(bytes, encoding, $"TOTAL: {total:0.00}€\n");
            AddText(bytes, encoding, "Gracies per la compra!\n");

            AddBytes(bytes, 0x1B, 0x64, 0x03);       // espai
            AddBytes(bytes, 0x1D, 0x56, 0x00);       // tall

            RawPrinterHelper.SendBytes(PrinterName, bytes.ToArray());
        }

        public void PrintSampleTicket()
        {
            var encoding = Encoding.GetEncoding(1252);
            var bytes = new List<byte>();

            AddBytes(bytes, 0x1B, 0x40);           

            AddBytes(bytes, 0x1B, 0x61, 0x01);       
            AddText(bytes, encoding, "TICKET VENTA\n");
            AddText(bytes, encoding, new string('-', LineWidth) + "\n");

            AddBytes(bytes, 0x1B, 0x61, 0x00);     
            AddText(bytes, encoding, $"Data: {DateTime.Now:dd/MM/yyyy HH:mm}\n");
            AddText(bytes, encoding, "\n");
            AddText(bytes, encoding, "Producte       Quan    Preu\n");
            AddText(bytes, encoding, new string('-', LineWidth) + "\n");
            AddText(bytes, encoding, "Article 1      x1     5,00€\n");
            AddText(bytes, encoding, "Article 2      x2     3,50€\n");
            AddText(bytes, encoding, new string('-', LineWidth) + "\n");
            AddText(bytes, encoding, "TOTAL                12,00€\n\n");

            AddBytes(bytes, 0x1B, 0x61, 0x01);     
            AddText(bytes, encoding, "Ticket de prova\n");
            AddText(bytes, encoding, "Gràcies per la compra!\n");

            AddBytes(bytes, 0x1B, 0x64, 0x03);       
            AddBytes(bytes, 0x1D, 0x56, 0x00);      

            RawPrinterHelper.SendBytes(PrinterName, bytes.ToArray());
        }

        public void PrintTicketCuina(EnviarCuinaDTO dto)
        {
            var encoding = Encoding.GetEncoding(1252);
            var bytes = new List<byte>();

            AddBytes(bytes, 0x1B, 0x40);            

            // TAULA al mig i gran
            AddBytes(bytes, 0x1B, 0x61, 0x01);      
            AddBytes(bytes, 0x1D, 0x21, 0x11);      
            AddText(bytes, encoding, $"TAULA: {dto.IdTaula}\n");
            AddBytes(bytes, 0x1D, 0x21, 0x00);  

            AddText(bytes, encoding, "*** CUINA ***\n");
            AddText(bytes, encoding, $"{DateTime.Now:dd/MM/yyyy HH:mm}\n");
            AddText(bytes, encoding, new string('-', LineWidth) + "\n");

            AddBytes(bytes, 0x1B, 0x61, 0x00);

            PrintSeccioCuina(bytes, encoding, "PRIMERS", dto.PrimersPlats);
            if(dto.SegonsPlats.Count > 0) PrintSeccioCuina(bytes, encoding, "SEGONS", dto.SegonsPlats);

            AddBytes(bytes, 0x1B, 0x64, 0x04);      
            AddBytes(bytes, 0x1D, 0x56, 0x00);      

            RawPrinterHelper.SendBytes(KitchenPrinterName, bytes.ToArray());
        }

        
       

        private static void PrintSeccioCuina(List<byte> bytes, Encoding encoding, string titol, List<PlatsCuinaDTO> plats)
        {
            AddText(bytes, encoding, titol + ":\n");
            AddText(bytes, encoding, new string('-', LineWidth) + "\n");

            if (plats == null || plats.Count == 0)
            {
                AddText(bytes, encoding, "—\n");
                AddText(bytes, encoding, new string('-', LineWidth) + "\n");
                return;
            }

            for (int i = 0; i < plats.Count; i++)
            {
                var p = plats[i];

                string nom = (p.NomPlat ?? "").Trim();
                string obs = (p.Observacions ?? "").Trim();

                AddText(bytes, encoding, FormatNomQty(nom, p.Quantitat));

                if (!string.IsNullOrWhiteSpace(obs))
                {
                    var lines = WrapText(obs, LineWidth - 4);
                    for (int j = 0; j < lines.Count; j++)
                    {
                        AddText(bytes, encoding,  lines[j] + "\n");
                    }
                }
            }

            AddText(bytes, encoding, new string('-', LineWidth) + "\n");
        }

        private static string FormatNomQty(string nom, int qty)
        {
            string qtyStr = "x" + qty;
            int maxNom = Math.Max(0, LineWidth - qtyStr.Length - 1);

            string nomTrim = nom.Length > maxNom ? nom.Substring(0, maxNom) : nom;
            return nomTrim.PadRight(maxNom) + " " + qtyStr + "\n";
        }

        private static List<string> WrapText(string text, int max)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(text)) return result;

            text = text.Replace("\r", "").Replace("\n", " ").Trim();
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string line = "";
            for (int i = 0; i < words.Length; i++)
            {
                var w = words[i];

                if (line.Length == 0)
                {
                    line = w.Length > max ? w.Substring(0, max) : w;
                }
                else if (line.Length + 1 + w.Length <= max)
                {
                    line += " " + w;
                }
                else
                {
                    result.Add(line);
                    line = w.Length > max ? w.Substring(0, max) : w;
                }
            }

            if (line.Length > 0) result.Add(line);
            return result;
        }

        private static void AddBytes(List<byte> bytes, params byte[] data)
        {
            bytes.AddRange(data);
        }

        private static void AddText(List<byte> bytes, Encoding encoding, string text)
        {
            bytes.AddRange(encoding.GetBytes(text));
        }

        private string FormatLinia(string nom, int qty, double total)
        {
            string nomTrim = nom.Length > 18 ? nom.Substring(0, 18) : nom;
            string colNom = nomTrim.PadRight(18);
            string colQty = ("x" + qty).PadLeft(4);
            string colTotal = (total.ToString("0.00") + "€").PadLeft(10);

            return $"{colNom}{colQty}{colTotal}\n";
        }
    }
}
