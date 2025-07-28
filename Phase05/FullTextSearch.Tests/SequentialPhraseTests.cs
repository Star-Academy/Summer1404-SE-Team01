//using FluentAssertions;
//using FullTextSearch.InvertedIndex.Dtos;

//namespace FullTextSearch.Tests;

//public class SequentialPhraseTests
//{
//    [Fact]
//    public void ValidateSequentiality_ShouldReturnsCommonDocs_WhenPhraseIsSequential()
//    {
//        var dto = CreateDto();
//        var input = new List<string> { "CODE", "STAR" };
//        var spv = new SequentialPhraseValidator();
//        var tempDocs = new SortedSet<string> { "doc1", "doc2", "doc3" };

//        spv.FindConsecutivePhrases(tempDocs, input, dto);

//        tempDocs.Should().HaveCount(2).And.Equal(new SortedSet<string> { "doc2", "doc3" });


//    }

//    private static InvertedIndexDto CreateDto()
//    {
//        var dto = new InvertedIndexDto
//        {
//            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5"],
//            InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>
//            {
//                ["CODE"] = new SortedSet<DocumentInfo>
//                {
//                    new DocumentInfo
//                    {
//                        DocId = "doc1",
//                        Indexes = { 33, 39, 27 }
//                    },
//                    new DocumentInfo
//                    {
//                        DocId = "doc2",
//                        Indexes = { 3, 39, 27 }
//                    },
//                    new DocumentInfo
//                    {
//                        DocId = "doc3",
//                        Indexes = { 8, 10, 12 }
//                    },
//                    new DocumentInfo
//                    {
//                        DocId = "doc4",
//                        Indexes = { 9, 40, 52}
//                    },
//                },

//                ["STAR"] = new SortedSet<DocumentInfo>
//                {
//                    new DocumentInfo
//                    {
//                        DocId = "doc1",
//                        Indexes = { 1,2,3,4,5 }
//                    },
//                    new DocumentInfo
//                    {
//                        DocId = "doc2",
//                        Indexes = { 4, 10, 12 }
//                    },
//                    new DocumentInfo
//                    {
//                        DocId = "doc3",
//                        Indexes = { 9, 40, 52}
//                    },
//                }
//            }
//        };

//        return dto;
//    }
//}