using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;                                      // כדי להשתמש ב-ParkingDbContext ובמחלקות הטבלאות
using DTO;                                      // כדי להשתמש ב-DriverDTO
using AutoMapper.QueryableExtensions;           // נותן את ProjectTo (המרה ישירות בשאילתה)
using AutoMapper;                               // הספרייה שממירה בין טבלה ל-DTO


namespace BLL.Repositories
{
    public class DriverRepository: IDriverRepository // המחלקה מממשת את כל מה שהממשק הגדיר
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;               // הכלי שמבצע המרות בין טבלה ל-DTO


        public DriverRepository(ParkingDbContext context, IMapper mapper)// בנאי: ה-DI מזריק לכאן את שני הכלים
        {
            _context = context;                     // שומר את החיבור למסד בשדה של המחלקה
            _mapper = mapper;                       // שומר את הממיר בשדה של המחלקה

        }   



    }
}
