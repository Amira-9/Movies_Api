namespace Movies_Api.Services
{
    public interface IGenresServices
    {
        Task<IEnumerable<Genre>> GetAll();
        Task <Genre> GetById(byte id);
        Task<Genre> Add(Genre genre);
        Genre Update(Genre genre);
        Genre Delete(Genre genre);
    }
}
