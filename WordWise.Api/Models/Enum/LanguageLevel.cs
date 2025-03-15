using System.ComponentModel;

namespace WordWise.Api.Models.Enum
{
    public enum LanguageLevel
    {
        [Description("Beginner: Hiểu các thông điệp rõ ràng và cụ thể trong thông tin cơ bản về cá nhân, hoặc những câu đơn giản trong các nội dung rất ngắn")]
        Beginner = 1,

        [Description("Elementary: Hiểu các câu và các cụm từ thông dụng liên quan đến các lĩnh vực quen thuộc (ví dụ: thông tin cá nhân cơ bản, gia đình, mua sắm, địa lý địa phương, việc làm).")]
        Elementary = 2,

        [Description("Intermediate: Hiểu những điểm chính trong các đoạn văn bản rõ ràng, tiêu chuẩn về các vấn đề quen thuộc thường gặp trong công việc, trường học, giải trí, v.v. Có thể xử lý hầu hết các tình huống có thể phát sinh khi đi du lịch ở khu vực có ngôn ngữ đó được sử dụng.")]
        Intermediate = 3,

        [Description("UpperIntermediate: Hiểu ý chính của văn bản phức tạp về cả chủ đề cụ thể và trừu tượng, bao gồm cả các cuộc thảo luận kỹ thuật trong lĩnh vực chuyên môn của mình. Có thể tương tác với mức độ lưu loát và tự nhiên, giúp tương tác thường xuyên với người bản ngữ mà không gây căng thẳng cho cả hai bên.")]
        UpperIntermediate = 4,

        [Description("Advanced: Hiểu một loạt các văn bản dài hơn, đòi hỏi cao, và nhận ra ý nghĩa ngụ ý. Có thể diễn đạt bản thân một cách trôi chảy và tự nhiên mà không cần tìm kiếm từ ngữ rõ ràng. Có thể sử dụng ngôn ngữ linh hoạt và hiệu quả cho các mục đích xã hội, học thuật và nghề nghiệp.")]
        Advanced = 5,

        [Description("Proficient: Có thể dễ dàng hiểu hầu như mọi thứ được nghe hoặc đọc. Có thể tóm tắt thông tin từ các nguồn nói hoặc viết khác nhau, xây dựng lại các lập luận và tường thuật trong một bản trình bày mạch lạc. Có thể diễn đạt bản thân một cách tự nhiên, rất trôi chảy và chính xác, phân biệt các sắc thái ý nghĩa tinh tế ngay cả trong những tình huống phức tạp nhất.")]
        Proficient = 6
    }
}
