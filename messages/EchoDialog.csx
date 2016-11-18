using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;
    protected int numOfGuess;
    protected int numOfGames = 0;
    protected long answer;
    protected GuessingGame game;

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
        context.PostAsync("Welcome to the Guessing Game bot");
        PromptDialog.Number(
            context,
            AfterSetMaximumAsync,
            "Give a positive integer",
            "Didn't get that!");
    }

    // public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
    // {
    //     var confirm = await argument;
    //     if (confirm)
    //     {
    //         this.count = 1;
    //         await context.PostAsync("Reset count.");
    //     }
    //     else
    //     {
    //         await context.PostAsync("Did not reset count.");
    //     }
    //     context.Wait(MessageReceivedAsync);
    // }
    
    
    public async Task AfterSetMaximumAsync(IDialogContext context, IAwaitable<long> argument)
    {
        var maxNum = await argument;

        if (maxNum is long)
        {
            
            await context.PostAsync("Now please guess the number");
            game = new GuessingGame((int) maxNum);

            PromptDialog.Number(
                context,
                AfterGuessAsync,
                "Give a positive integer",
                "Didn't get that!");
        }
        else
        {
            await context.PostAsync("Please input an integer.");
        }
        
        context.Wait(MessageReceivedAsync);
    }

    public async Task AfterGuessAsync(IDialogContext context, IAwaitable<long> argument)
    {
        var guess = await argument;

        if (guess is long)
        {
            Result result = game.Guess((int) guess);
            if (result == Result.Correct)
            {
                await context.PostAsync("You have guessed the right number!");
                context.Wait(MessageReceivedAsync);

            }
            else if (result == Result.TooHigh)
            {
                await context.PostAsync("Guess is too high");
                context.Wait(AfterGuessAsync);
            }
            else if (result == Result.TooLow)
            {
                await context.PostAsync("Guess is too low");
                context.Wait(AfterGuessAsync);
            }
            else
            {
                await context.PostAsync("You're a quitter...");
                context.Wait(MessageReceivedAsync);
            }
        }
        else
        {
            await context.PostAsync("Please input an integer.");
            context.Wait(AfterGuessAsync);
        }
    }
}