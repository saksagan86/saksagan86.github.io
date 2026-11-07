using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface IBlogpostService
{
    Task<List<BlogpostDto>> GetAllAsync();
    Task<BlogpostDto?> GetByIdAsync(int id);
    Task<BlogpostDto> CreateAsync(CreateBlogpostDto dto);
    Task<BlogpostDto?> UpdateAsync(int id, UpdateBlogpostDto dto);
    Task<bool> DeleteAsync(int id);
}