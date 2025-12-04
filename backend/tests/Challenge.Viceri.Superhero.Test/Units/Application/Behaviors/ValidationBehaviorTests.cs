//using Challenge.Viceri.Superhero.Application.Behaviors;
//using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
//using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
//using FluentAssertions;
//using FluentValidation;
//using FluentValidation.Results;
//using Moq;

//namespace Challenge.Viceri.Superhero.Test.Units.Application.Behaviors;

//public class ValidationBehaviorTests
//{
//    [Fact]
//    public async Task Handle_WithNoValidators_ShouldCallNext()
//    {
//        // Arrange
//        var validators = Array.Empty<IValidator<CreateHeroCommand>>();
//        var behavior = new ValidationBehavior<CreateHeroCommand, HeroDto>(validators);

//        var command = new CreateHeroCommand(
//            "Peter Parker",
//            "Homem-Aranha",
//            new List<int> { 1 },
//            null,
//            1.78f,
//            76.5f
//        );

//        var nextCalled = false;
//        Task<HeroDto> Next()
//        {
//            nextCalled = true;
//            return Task.FromResult(new HeroDto
//            {
//                Id = 1,
//                Name = "Peter Parker",
//                Codename = "Homem-Aranha",
//                SuperPowers = []
//            });
//        }

//        // Act
//        var result = await behavior.Handle(command, Next, CancellationToken.None);

//        // Assert
//        nextCalled.Should().BeTrue();
//        result.Should().NotBeNull();
//    }

//    [Fact]
//    public async Task Handle_WithValidCommand_ShouldCallNext()
//    {
//        // Arrange
//        var validatorMock = new Mock<IValidator<CreateHeroCommand>>();
//        validatorMock
//            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHeroCommand>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ValidationResult());

//        var behavior = new ValidationBehavior<CreateHeroCommand, HeroDto>(new[] { validatorMock.Object });

//        var command = new CreateHeroCommand(
//            "Peter Parker",
//            "Homem-Aranha",
//            new List<int> { 1 },
//            null,
//            1.78f,
//            76.5f
//        );

//        var nextCalled = false;
//        Task<HeroDto> Next()
//        {
//            nextCalled = true;
//            return Task.FromResult(new HeroDto
//            {
//                Id = 1,
//                Name = "Peter Parker",
//                Codename = "Homem-Aranha",
//                SuperPowers = []
//            });
//        }

//        // Act
//        var result = await behavior.Handle(command, Next, CancellationToken.None);

//        // Assert
//        nextCalled.Should().BeTrue();
//        result.Should().NotBeNull();
//        validatorMock.Verify(v => v.ValidateAsync(
//            It.IsAny<ValidationContext<CreateHeroCommand>>(),
//            It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task Handle_WithInvalidCommand_ShouldThrowValidationException()
//    {
//        // Arrange
//        var validationFailures = new List<ValidationFailure>
//        {
//            new ValidationFailure("Name", "O nome é obrigatório.") { ErrorCode = "Hero.Name.Required" },
//            new ValidationFailure("Codename", "O alter ego é obrigatório.") { ErrorCode = "Hero.Codename.Required" }
//        };

//        var validatorMock = new Mock<IValidator<CreateHeroCommand>>();
//        validatorMock
//            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHeroCommand>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ValidationResult(validationFailures));

//        var behavior = new ValidationBehavior<CreateHeroCommand, HeroDto>(new[] { validatorMock.Object });

//        var command = new CreateHeroCommand(
//            "",
//            "",
//            new List<int> { 1 },
//            null,
//            1.78f,
//            76.5f
//        );

//        var nextCalled = false;
//        Task<HeroDto> Next()
//        {
//            nextCalled = true;
//            return Task.FromResult(new HeroDto());
//        }

//        // Act
//        Func<Task> act = async () => await behavior.Handle(command, Next, CancellationToken.None);

//        // Assert
//        await act.Should().ThrowAsync<ValidationException>()
//            .WithMessage("*validation*");

//        nextCalled.Should().BeFalse();
//    }

//    [Fact]
//    public async Task Handle_WithMultipleValidators_ShouldAggregateErrors()
//    {
//        // Arrange
//        var validator1Mock = new Mock<IValidator<CreateHeroCommand>>();
//        validator1Mock
//            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHeroCommand>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ValidationResult(new[]
//            {
//                new ValidationFailure("Name", "Nome inválido")
//            }));

//        var validator2Mock = new Mock<IValidator<CreateHeroCommand>>();
//        validator2Mock
//            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHeroCommand>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ValidationResult(new[]
//            {
//                new ValidationFailure("Height", "Altura inválida")
//            }));

//        var behavior = new ValidationBehavior<CreateHeroCommand, HeroDto>(
//            new[] { validator1Mock.Object, validator2Mock.Object });

//        var command = new CreateHeroCommand(
//            "",
//            "Homem-Aranha",
//            new List<int> { 1 },
//            null,
//            0,
//            76.5f
//        );

//        Task<HeroDto> Next() => Task.FromResult(new HeroDto());

//        // Act
//        Func<Task> act = async () => await behavior.Handle(command, Next, CancellationToken.None);

//        // Assert
//        var exception = await act.Should().ThrowAsync<ValidationException>();
//        exception.Which.Errors.Should().HaveCount(2);
//    }

//    [Fact]
//    public async Task Handle_WithCancellationToken_ShouldPassTokenToValidators()
//    {
//        // Arrange
//        var cancellationToken = new CancellationToken();
//        var validatorMock = new Mock<IValidator<CreateHeroCommand>>();
//        validatorMock
//            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHeroCommand>>(), cancellationToken))
//            .ReturnsAsync(new ValidationResult());

//        var behavior = new ValidationBehavior<CreateHeroCommand, HeroDto>(new[] { validatorMock.Object });

//        var command = new CreateHeroCommand(
//            "Peter Parker",
//            "Homem-Aranha",
//            new List<int> { 1 },
//            null,
//            1.78f,
//            76.5f
//        );

//        Task<HeroDto> Next() => Task.FromResult(new HeroDto());

//        // Act
//        await behavior.Handle(command, Next, cancellationToken);

//        // Assert
//        validatorMock.Verify(v => v.ValidateAsync(
//            It.IsAny<ValidationContext<CreateHeroCommand>>(),
//            cancellationToken), Times.Once);
//    }

//    [Fact]
//    public async Task Handle_WithPartialValidationFailures_ShouldThrowWithAllErrors()
//    {
//        // Arrange
//        var validationFailures = new List<ValidationFailure>
//        {
//            new ValidationFailure("Height", "Altura deve ser maior que zero") { ErrorCode = "Hero.Height.Invalid" },
//            new ValidationFailure("Weight", "Peso deve ser maior que zero") { ErrorCode = "Hero.Weight.Invalid" },
//            new ValidationFailure("SuperpowerIds", "Lista vazia") { ErrorCode = "Hero.Superpowers.Empty" }
//        };

//        var validatorMock = new Mock<IValidator<CreateHeroCommand>>();
//        validatorMock
//            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHeroCommand>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));

//        var behavior = new ValidationBehavior<CreateHeroCommand, HeroDto>(new[] { validatorMock.Object });

//        var command = new CreateHeroCommand(
//            "Peter Parker",
//            "Homem-Aranha",
//            new List<int>(),
//            null,
//            0,
//            0
//        );

//        Task<HeroDto> Next() => Task.FromResult(new HeroDto());

//        // Act
//        Func<Task> act = async () => await behavior.Handle(command, Next, CancellationToken.None);

//        // Assert
//        var exception = await act.Should().ThrowAsync<ValidationException>();
//        exception.Which.Errors.Should().HaveCount(3);
//        exception.Which.Errors.Should().Contain(e => e.PropertyName == "Height");
//        exception.Which.Errors.Should().Contain(e => e.PropertyName == "Weight");
//        exception.Which.Errors.Should().Contain(e => e.PropertyName == "SuperpowerIds");
//    }
//}