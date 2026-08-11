using Pertamina.Services.UserPositions;

namespace EBVL.FrontEnd.WebUi.Common.Components;

public partial class DialogSwitchPosition
{
    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; init; }

    [Inject]
    public required IUserPositionsService UserPositionsService { get; init; }

    private IEnumerable<PositionModel> _positions = [];
    private PositionModel? _currentPosition;
    private PositionModel _selectedPosition = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadPositions();
    }

    private async Task LoadPositions()
    {
        try
        {
            _isLoading = true;

            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            var userId = user.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidOperationException("User ID is not available.");
            }

            var getUserPositionsResult = await UserPositionsService.GetUserPositions(userId);

            _positions = getUserPositionsResult.Positions.Select(position => new PositionModel
            {
                Id = position.Id,
                Name = position.Name
            });

            if (!_positions.Any())
            {
                return;
            }

            var currentPositionId = user.GetPositionId();

            if (string.IsNullOrWhiteSpace(currentPositionId))
            {
                _selectedPosition = _positions.First();

                return;
            }

            var currentPositionName = user.GetPositionName();

            if (string.IsNullOrWhiteSpace(currentPositionName))
            {
                currentPositionName = "Unknown Position";
            }

            _currentPosition = new PositionModel
            {
                Id = currentPositionId,
                Name = currentPositionName
            };

            var selectedPosition = _positions.FirstOrDefault(x => x.Id == _currentPosition.Id);

            if (selectedPosition is null)
            {
                _selectedPosition = _positions.First();
                return;
            }

            _selectedPosition = selectedPosition;
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            await Task.CompletedTask;

            ClearException();

            Dialog.Close(DialogResult.Ok(_selectedPosition.Id));
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private sealed record PositionModel
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
    }
}
