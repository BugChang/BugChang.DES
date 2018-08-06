using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Letters;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class LetterRepository : BaseRepository<Letter>, ILetterRepository
    {
        private readonly DesDbContext _dbContext;
        public LetterRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<PageResultModel<Letter>> GetTodayReceiveLetters(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment).Include(a => a.CreateUser).Where(a =>
                      a.LetterType == EnumLetterType.收信 && a.CreateTime.Date == DateTime.Now.Date);

            return new PageResultModel<Letter>
            {
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }

        public async Task<PageResultModel<Letter>> GetTodaySendLetters(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment).Include(a => a.CreateUser).Where(a =>
                (a.LetterType == EnumLetterType.发信 || a.LetterType == EnumLetterType.内交换) && a.SendDepartmentId == pageSearchModel.DepartmentId && a.CreateTime.Date == DateTime.Now.Date);

            return new PageResultModel<Letter>
            {
                Rows = await query.OrderByDescending(a => a.Id).Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync(),
                Total = await query.CountAsync()
            };
        }

        public async Task<PageResultModel<Letter>> GetReceiveLetters(LetterPageSerchModel pageSearch)
        {
            //父级单位可以查询子单位的信件
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment)
                .Include(a => a.CreateUser)
                .Join(_dbContext.BarcodeLogs, l => l.ReceiveDepartmentId, bl => bl.DepartmentId, (l, bl) => new { l, bl })
                .Where(a => a.bl.BarcodeStatus == EnumBarcodeStatus.已签收);
            if (pageSearch.BeginTime != null)
            {
                query = query.Where(a => a.bl.OperationTime >= pageSearch.BeginTime.Value);
            }

            if (pageSearch.EndTime != null)
            {
                query = query.Where(a => a.bl.OperationTime <= pageSearch.EndTime.Value);
            }

            if (!string.IsNullOrWhiteSpace(pageSearch.LetterNo))
            {
                query = query.Where(a => a.l.LetterNo.Contains(pageSearch.LetterNo));
            }

            if (pageSearch.SendDepartmentId != 0)
            {
                query = query.Where(a => a.l.SendDepartmentId == pageSearch.SendDepartmentId);
            }
            if (!string.IsNullOrWhiteSpace(pageSearch.ShiJiNo))
            {
                query = query.Where(a => a.l.ShiJiCode.Contains(pageSearch.ShiJiNo));
            }
            var newQuery = query.Select(a => a.l);

            return new PageResultModel<Letter>
            {
                Rows = await newQuery.Skip(pageSearch.Skip).Take(pageSearch.Take).ToListAsync(),
                Total = await newQuery.CountAsync()
            };
        }

        public async Task<PageResultModel<Letter>> GetManagerReceiveLetters(LetterPageSerchModel pageSearch)
        {
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment).Include(a => a.CreateUser).Where(a => a.LetterType == EnumLetterType.收信);
            if (pageSearch.BeginTime != null)
            {
                query = query.Where(a => a.CreateTime >= pageSearch.BeginTime.Value);
            }

            if (pageSearch.EndTime != null)
            {
                query = query.Where(a => a.CreateTime <= pageSearch.EndTime.Value);
            }

            if (!string.IsNullOrWhiteSpace(pageSearch.LetterNo))
            {
                query = query.Where(a => a.LetterNo.Contains(pageSearch.LetterNo));
            }

            if (pageSearch.SendDepartmentId != 0)
            {
                query = query.Where(a => a.SendDepartmentId == pageSearch.SendDepartmentId);
            }
            if (!string.IsNullOrWhiteSpace(pageSearch.ShiJiNo))
            {
                query = query.Where(a => a.ShiJiCode.Contains(pageSearch.ShiJiNo));
            }
            return new PageResultModel<Letter>
            {
                Rows = await query.Skip(pageSearch.Skip).Take(pageSearch.Take).ToListAsync(),
                Total = await query.CountAsync()
            };
        }

        public async Task<Letter> GetLetter(string barcodeNo)
        {
            var letter = await _dbContext.Letters.AsNoTracking().Where(a => a.BarcodeNo == barcodeNo).SingleOrDefaultAsync();
            return letter;
        }

        public async Task<PageResultModel<Letter>> GetSendLetters(LetterPageSerchModel pageSearch)
        {
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment).Include(a => a.CreateUser).Where(a => a.LetterType != EnumLetterType.收信 && a.SendDepartmentId == pageSearch.SendDepartmentId);
            if (pageSearch.BeginTime != null)
            {
                query = query.Where(a => a.CreateTime >= pageSearch.BeginTime.Value);
            }

            if (pageSearch.EndTime != null)
            {
                query = query.Where(a => a.CreateTime <= pageSearch.EndTime.Value);
            }
            if (!string.IsNullOrWhiteSpace(pageSearch.LetterNo))
            {
                query = query.Where(a => a.LetterNo.Contains(pageSearch.LetterNo));
            }

            if (pageSearch.ReceiveDepartmentId != 0)
            {
                query = query.Where(a => a.ReceiveDepartmentId == pageSearch.ReceiveDepartmentId);
            }
            return new PageResultModel<Letter>
            {
                Rows = await query.Skip(pageSearch.Skip).Take(pageSearch.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }

        public async Task<PageResultModel<Letter>> GetBackLettersForSearch(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment).Include(a => a.CreateUser).Where(a => a.ReceiveDepartmentId == pageSearchModel.DepartmentId && a.BarcodeNo.Contains(pageSearchModel.Keywords));
            return new PageResultModel<Letter>
            {
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).OrderByDescending(a => a.Id).ToListAsync(),
                Total = await query.CountAsync()
            };
        }

        public async Task<PageResultModel<Letter>> GetBackLettersForManagerSearch(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Letters.Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment).Include(a => a.CreateUser).Where(a => a.BarcodeNo.Contains(pageSearchModel.Keywords));
            return new PageResultModel<Letter>
            {
                Rows = await query.OrderByDescending(a => a.Id).Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync(),
                Total = await query.CountAsync()
            };
        }
    }

    public class BackLetterRepository : BaseRepository<BackLetter>, IBackLetterRepository
    {
        private readonly DesDbContext _dbContext;
        public BackLetterRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResultModel<Letter>> GetBackLetters(PageSearchCommonModel pageSerch)
        {
            var query = from letter in _dbContext.Letters.Include(a => a.CreateUser).Include(a => a.SendDepartment).Include(a => a.ReceiveDepartment)
                        join backLetter in _dbContext.BackLetters on letter.Id equals backLetter.LetterId
                        where backLetter.OperationDepartmentId == pageSerch.DepartmentId
                        select letter;
            if (!string.IsNullOrWhiteSpace(pageSerch.Keywords))
            {
                query = query.Where(a => a.LetterNo.Contains(pageSerch.Keywords));
            }

            return new PageResultModel<Letter>
            {
                Rows = await query.OrderByDescending(a => a.Id).Skip(pageSerch.Skip).Take(pageSerch.Take).ToListAsync(),
                Total = await query.Select(a => a).CountAsync()
            };
        }
    }

    public class CancelLetterRepository : BaseRepository<CancelLetter>, ICancelLetterRepository
    {
        public CancelLetterRepository(DesDbContext dbContext) : base(dbContext)
        {
        }
    }
}
