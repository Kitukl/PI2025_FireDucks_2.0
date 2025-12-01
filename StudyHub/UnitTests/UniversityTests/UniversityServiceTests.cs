using Moq;
using StudyHub.BLL.Services;
using Xunit;

public class UniversityServiceTests
{
    [Fact]
    public void GetFacultyByGroup_ShouldReturnCorrectFaculty_WhenMatchFound()
    {
        // Arrange
        var mockFaculty1 = new Mock<IFacultyHelper>();
        mockFaculty1.Setup(f => f.IsMatch("ПМІ-31с")).Returns(true);

        var mockFaculty2 = new Mock<IFacultyHelper>();
        mockFaculty2.Setup(f => f.IsMatch("ПМІ-31с")).Returns(false);

        var faculties = new List<IFacultyHelper> { mockFaculty2.Object, mockFaculty1.Object };
        var service = new UniversityService(faculties);

        // Act
        var result = service.GetFacultyByGroup("ПМІ-31с");

        // Assert
        Assert.Same(mockFaculty1.Object, result);
    }

    [Fact]
    public void GetFacultyByGroup_ShouldThrowException_WhenNoMatchFound()
    {
        // Arrange
        var mockFaculty = new Mock<IFacultyHelper>();
        mockFaculty.Setup(f => f.IsMatch(It.IsAny<string>())).Returns(false);

        var service = new UniversityService(new[] { mockFaculty.Object });

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.GetFacultyByGroup("UNKNOWN-11"));
    }

    [Fact]
    public void GetFacultyByGroup_ShouldThrowArgumentException_WhenGroupNameIsEmpty()
    {
        var service = new UniversityService(Enumerable.Empty<IFacultyHelper>());
        Assert.Throws<ArgumentException>(() => service.GetFacultyByGroup(""));
    }
}