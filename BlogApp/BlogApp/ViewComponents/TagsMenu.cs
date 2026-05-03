using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    // ********************************* View Component ****************************
    // Biz Tag modeli'inin listesini bütün sayfalarda görüntülemek istiyoruz. Bunun için bir partial view açıp, her sayfa için model göndererek yapabiliriz. Ancak partial view her action'dan Tags List'ini model olarak göndermemiz gerekli. Ancak biz bunu istemiyoruz. Bunun içinde View Components'i(Bileşenleri Görüntüle) kullancağız.
    // View Components nedir?: Birden fazla veya her sayfada görüntülemek istenen veriler var. Ancak biz bu verileri her controller'dan ve her action'dan model olarak göndermek istemiyoruz. Bunun içinde ayrı bir controller gibi bir class açıyoruz. Bu class'ı ViewComponent class'ından türetiyoruz. Daha sonra verilerimizi çekeceğimiz type'ın instance'ını alıyoruz. Daha sonra IViewComponentResult türünde bir metot tanımlıyoruz. Bu metot bir action metot benzeri çalışıyor. Bu metotdan db'den Tags'lerin listesini alarak partial view'e return ederek gönderiyoruz. Hangi partial view'e gidiyor: Shared altında, Components altında, TagsMenu klasörleri altındaki Default.cshtml view'ine gönderiliyor. 
    // Default.cshtml sayfasına git...
    // Bu işlemler yapıldıktan sonra, bu partial view'i çağırma işlemi yapılabilir. Nasıl yapacağız?: bu sayfanın çağırılacağı view sayfasına gelip, <vc:sayfa-ismi><vc:sayfa-ismi/>(vc:View Components'in kısaltılmışı, tags-menu'de partial view sayfasının ismi. Sayfanın 2 ismi varsa araya - koyarak yazılmalı. Eğer bu taghelper başta çalışmazsa _ViewImport.cshtml sayfasına gidip, et işareti ile addTagHelper *, BlogApp yazmamız gerekli. Bununla programa BlogApp projesinin tüm dosyalarında taghelper kullanımı açmış oluyoruz.) bu şekilde bir taghelper yardmıyla view components sayfamız çağırılmış oluyor.
    // Posts.Index.cshtml sayfasına git...

    // ViewComponent class'ı ile dışarıdan bir controller açmış oluyoruz.

    public class TagsMenu : ViewComponent
    {
        private ITagRepository _tagRepository;
        public TagsMenu(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        
        {
            return View(await _tagRepository.GetAll.ToListAsync()); // Burada önemli bir nokta var. parametredeki model tanımlamasından önce bir view ismi girmezsek, bu model bu class'ın isiminde açılmış olan klasörün altında, Default adında ki partial view sayfasına gönderilir. Eğer biz farklı bir view'e göndermek istersek isim tanımlaması yapmamız gerekli.   
        }

    }
}
