using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Core.Enum
{

    //Enum, sabit bir seçenek listesini temsil eden bir tiptir. Yani "bu değer sadece şu belirli seçeneklerden biri olabilir" demenin yolu.Bunu neden kullanıyoruz? Çünkü kargonun durumu serbest metin olmamalı — biri yanlışlıkla "yolda", "Yolda " (fazladan boşluk), "YOLDA" yazarsa, kodun içinde "eğer durum Yolda ise mail gönder" gibi karşılaştırmalar tutarsız çalışır.
    public enum KargoDurum
    {
        Hazirlaniyor,
        KargoyaVerildi,
        Yolda,
        TeslimEdildi,
        IadeSurecinde,
        IadeEdildi
    }
}
