#load "GuessingGame.csx"

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    
    protected List<GuessingGame> games = new List<GuessingGame>();

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        //await context.PostAsync($"{this.count++}: You said {message.Text}");
        await context.PostAsync($"Hello, {message.From.Name}");

        ShowStartMenu(context);
       
    }

    private void ShowStartMenu(IDialogContext context)
    {
        String[] options =
        {
            "restart",
            "report",
            "clearhistory"
        };

        String[] optionsDescriptions =
        {
            "Start a new game",
            "Report my stat",
            "Clear my history"
        };

        PromptDialog.Choice(
            context,
            AfterStartMenuSelectedAsync,
            options,
            "Welcome to the Guessing Game bot",
            "Please select from one of the options",
            3,
            PromptStyle.Auto,
            optionsDescriptions);
    }

    public async Task AfterStartMenuSelectedAsync(IDialogContext context, IAwaitable<String> argument)
    {
        var selectedChoice = await argument;

        if (selectedChoice == "restart")
        {
            PromptDialog.Number(
                context,
                AfterSetMaximumAsync,
                "How large do you want your guess to be? Give me a positive integer number!",
                "Didn't get that!");
        }
        else if (selectedChoice == "report")
        {
            await ReportStat(context);
            ShowStartMenu(context);
        }
        else if (selectedChoice == "clearhistory")
        {
            games.Clear();
            await context.PostAsync("All game history cleared");
            ShowStartMenu(context);
        }
        else
        {
            ShowStartMenu(context);
        }
    }

    private async Task ReportStat(IDialogContext context)
    {
        

        var winningGuesses = from g in games
                             where g.IsWin
                             select g.NumOfGuess;

        if (winningGuesses.Any())
        {
            await context.PostAsync($"Your history\nTotal games played: {games.Count()}!");
            await context.PostAsync($"Average number of guesses: { ((double) winningGuesses.Sum() / winningGuesses.Count())}\nLowest number of guess: { winningGuesses.Min()}");
        }
        else
        {
            await context.PostAsync($"Your history\nTotal games played: {games.Count()}!");
            await context.PostAsync($"You haven't won a single game though....");
        }

    }

    public async Task AfterSetMaximumAsync(IDialogContext context, IAwaitable<long> argument)
    {
        var maxNum = await argument;

        //await context.PostAsync("Now please guess the number");
        games.Add(new GuessingGame((int) maxNum));

        PromptDialog.Number(
            context,
            AfterGuessAsync,
            "Now please make a guess of the number...",
            "Didn't get that!");
        
    }

    public async Task AfterFinishGameAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;

        if (confirm)
        {
            PromptDialog.Number(
                context,
                AfterSetMaximumAsync,
                "How large do you want your guess to be? Give me a positive integer number!",
                "Didn't get that!");
        }
        else
        {
            await ReportStat(context);
            ShowStartMenu(context);
        }
    }

    public async Task AfterGuessAsync(IDialogContext context, IAwaitable<long> argument)
    {
        var guess = await argument;
        var currentGame = games.Last();
        Result result = currentGame.Guess((int) guess);

        if (result == Result.Correct)
        {
            await context.PostAsync($"You have guessed the right number!\n You made {currentGame.NumOfGuess} guess in this round");
          

            PromptDialog.Confirm(
                context,
                AfterFinishGameAsync,
                "Do you want play another game?",
                "Didn't get that!",
                3,
                PromptStyle.Auto);

            //TODO: ask if they want to play again.

        }
        else if (result == Result.TooHigh)
        {
            PromptDialog.Number(
                context,
                AfterGuessAsync,
                "Guess is too high... try again",
                "Didn't get that!");
        }
        else if (result == Result.TooLow)
        {
            PromptDialog.Number(
                context,
                AfterGuessAsync,
                "Guess is too low... try again",
                "Didn't get that!");
        }
        else
        {
            await context.PostAsync("You're a quitter...");
            PromptDialog.Confirm(
                context,
                AfterFinishGameAsync,
                "Do you want play another game?",
                "Didn't get that!",
                3,
                PromptStyle.Auto);
        }

    }

}