using krediHesaplamaNew;

double krediTutari = 15000;
double faizOraniYillik = 0.54;
double faizOraniAylik = 0.04725; // + KKDF
double taksitSayisi = 6;
DateTime taksitTarihi = new DateTime(2024, 09, 18);

// kalan anapara ilk ay kredi tutarına eşittir.
double KA = krediTutari;

// ilk taksit tarihinden, son taksit tarihine kadar kaç gün olduğunu hesaplıyoruz.
DateTime ilkTaksitTarihi = taksitTarihi.AddMonths(1);
DateTime sonTaksitTarihi = ilkTaksitTarihi.AddMonths(6);
int toplamGunSayisi = (sonTaksitTarihi - ilkTaksitTarihi).Days - 1;

// T yani taksit tutarını hesapla
double T = krediTutari * (faizOraniAylik * Math.Pow(1 + faizOraniAylik, taksitSayisi)) / (Math.Pow(1 + faizOraniAylik, taksitSayisi) - 1);

List<anapara> anaparaOdemesi = new List<anapara>();

// i = 1, i = taksit sayısı olana kadar for döngüsünü devam ettir
for (int i = 1; i <= taksitSayisi; i++)
{
    // sonraki taksit tarihi, güncel taksit tarihindne 1 ay sonrasını al
    DateTime sonrakiTaksitTarihi = taksitTarihi.AddMonths(1);

    // sonraki taksit tarihi, güncel taksit tarihindne 30 gün sonrasını al
    //DateTime sonrakiTaksitTarihi = taksitTarihi.AddDays(30);

    // iki tarih arasındaki gün farkını hesapla
    TimeSpan gunFarki = sonrakiTaksitTarihi - taksitTarihi;

    // faiz hesaplama
    double F = KA * faizOraniYillik * gunFarki.Days / 360;

    // kkdf
    double KKDF = F * 0.05;

    // anapara hesaplama
    double A = T - (F + KKDF);

    // kalan anaparayı A'ya eşitle 
    KA -= A;

    // hesaplamadan sonra taksit tarihini ileri alıyoruz
    taksitTarihi = sonrakiTaksitTarihi;

    anaparaOdemesi.Add(new anapara
    {
        taksitTarihi1 = taksitTarihi,
        taksitTutari1 = T,
        anaparaTutari = A,
        faizTutari = F,
        kkdf = KKDF,
        kalanAnapara = KA,
        gunSayisi1 = gunFarki.TotalDays
    });

    Console.WriteLine($"{i}. Ay ({taksitTarihi.ToShortDateString()}):");
    Console.WriteLine($"Gün Aralığı: {gunFarki.Days} gün");
    Console.WriteLine($"Taksit Tutarı (T): {T:F2} TL");
    Console.WriteLine($"Faiz Tutarı (F{i}): {F:F2} TL");
    Console.WriteLine($"KKDF Tutarı (KKDF{i}): {KKDF:F2} TL");
    //Console.WriteLine($"Anapara (A{i}): {A:F2} TL");
    //Console.WriteLine($"Kalan Anapara (KA{i}): {KA:F2} TL");
    Console.WriteLine("--------------------------------------");

    // kalan tutardan günlük tutarı buluyoruz ve daha sonra gün sayısına göre anaparaya ekleme yapacağız(dağıtacağız)
    double gunlukTutar = KA / toplamGunSayisi;
    double guncelKA = krediTutari;

    foreach (var odeme in anaparaOdemesi)
    {
        // günlük tutarı hesapla
        double gunBazliEkTutar = gunlukTutar * odeme.gunSayisi1;
        // hesaplanan tutarı anaparaya ekle
        double guncelAnaparaOdemesi = odeme.anaparaTutari + gunBazliEkTutar;
        // kalan anaparadan yeni anaparayı çıkar
        guncelKA -= guncelAnaparaOdemesi;

        Console.WriteLine($"Anapara: {guncelAnaparaOdemesi:F2} TL");
        Console.WriteLine($"Kalan Anapara: {guncelKA:F2} TL");
        Console.WriteLine("--------------------------------------");
    }
}