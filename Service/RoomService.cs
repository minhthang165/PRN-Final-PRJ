using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        public async Task AddAsync(Room entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Room entity cannot be null.");
            }
            await _roomRepository.AddAsync(entity);
        }

        public Task DeleteAsync(int id)
        {
            var existingRoom = _roomRepository.GetByIdAsync(id);
            if (existingRoom == null)
            {
                throw new KeyNotFoundException($"Room with ID {id} not found.");
            }
            return _roomRepository.DeleteAsync(id);
        }

        public Task<List<Room>> GetAllAsync()
        {
            return _roomRepository.GetAllAsync();
        }

        public Task<Page<Room>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return _roomRepository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public Task<Room?> GetByIdAsync(int id)
        {
            return _roomRepository.GetByIdAsync(id);
        }


        public Task UpdateAsync(Room entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Room entity cannot be null.");
            }
            return _roomRepository.UpdateAsync(entity);
        }
        public async Task<ImportResult> ImportRoomsFromExcelAsync(IFormFile file)
        {
            var result = new ImportResult();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0);
                int rowCount = sheet.LastRowNum;

                for (int row = 1; row <= rowCount; row++)
                {
                    try
                    {
                        IRow excelRow = sheet.GetRow(row);
                        if (excelRow == null) continue;

                        var roomName = excelRow.GetCell(0)?.ToString();
                        var existingRoom = await _roomRepository.GetAllAsync();
                        if (existingRoom.Any(u => u.room_name.Equals(roomName)))
                        {
                            result.FailedCount++;
                            result.Errors.Add($"Row {row + 1}: '{roomName}' already exists.");
                            continue;
                        }
                        var room = new Room
                        {
                            room_name = roomName,
                            location = excelRow.GetCell(1)?.ToString(),
                            capicity = int.TryParse(excelRow.GetCell(2)?.ToString(), out int capacity) ? (int?)capacity : null,
                            created_at = System.DateTime.Now,
                            is_active = true
                        };
                        await _roomRepository.AddAsync(room);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailedCount++;
                        result.Errors.Add($"Row {row + 1}: {ex.Message}");
                    }
                }
            }
            return result;
        }

    }
}
