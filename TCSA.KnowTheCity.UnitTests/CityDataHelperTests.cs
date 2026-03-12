using TCSA.KnowTheCity.Core.Helpers;

namespace TCSA.KnowTheCity.UnitTests;

public class CityDataHelperTests
{
    // ── NormalizeForPath (via GetLandmarkImagePath) ───────────────────────────

    [Test]
    [TestCase("Sacré-Cœur", "sacrecoeur")]          // é → e, ligature œ → oe, hyphen stripped
    [TestCase("Arc de Triomphe", "arcdetriomphe")]   // spaces stripped
    [TestCase("Notre-Dame", "notredame")]             // hyphen stripped, plain ASCII
    [TestCase("Louvre Museum", "louvremuseum")]       // plain ASCII, space stripped
    [TestCase("Eiffel Tower", "eiffeltower")]         // plain ASCII, space stripped
    public void NormalizeForPath_FrenchLandmarks_ReturnsExpected(string input, string expected)
    {
        var result = CityDataHelper.NormalizeForPath(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    // ── GetLandmarkImagePath ──────────────────────────────────────────────────

    [Test]
    public void GetLandmarkImagePath_SacreCoeur_ReturnsCorrectPath()
    {
        var path = CityDataHelper.GetLandmarkImagePath("Paris", "Sacré-Cœur");
        Assert.That(path, Is.EqualTo("landmarks/paris-sacrecoeur.webp"));
    }

    [Test]
    public void GetLandmarkImagePath_ArcDeTriomphe_ReturnsCorrectPath()
    {
        var path = CityDataHelper.GetLandmarkImagePath("Paris", "Arc de Triomphe");
        Assert.That(path, Is.EqualTo("landmarks/paris-arcdetriomphe.webp"));
    }

    [Test]
    public void GetLandmarkImagePath_NotreDame_ReturnsCorrectPath()
    {
        var path = CityDataHelper.GetLandmarkImagePath("Paris", "Notre-Dame");
        Assert.That(path, Is.EqualTo("landmarks/paris-notredame.webp"));
    }

    // ── GetCityImagePath ──────────────────────────────────────────────────────

    [TestCase("New York", "newyork")]
    public void GetCityImagePath_Paris_ReturnsCorrectPath(string city, string result)
    {
        var path = CityDataHelper.GetCityImagePath(city);
        Assert.That(path, Is.EqualTo($"cities/{result}.webp"));
    }
}
