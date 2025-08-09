using LibraTrack.API.DTOs;

namespace LibraTrack.API.Helpers
{
    public class Pagination<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }
        public Pagination(int pageIndex , int pagesize ,int count , IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pagesize;
            Count = count;
            Data = data;
        }
    }
}
