#load "GuessingGame.csx"

using System;
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
        await context.PostAsync($"Welcome to the Guessing Game bot, {message.From.Name}");

   
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

        //await context.PostAsync("Now please guess the number");
        games.Add(new GuessingGame((int) maxNum));

        PromptDialog.Number(
            context,
            AfterGuessAsync,
            "Now please make a guess of the number...",
            "Didn't get that!");
        
    }


    public async Task AfterGuessAsync(IDialogContext context, IAwaitable<long> argument)
    {
        var guess = await argument;

        Result result = games.Last().Guess((int) guess);

        if (result == Result.Correct)
        {
            await context.PostAsync("You have guessed the right number!");
            context.Wait(MessageReceivedAsync);

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
            context.Wait(MessageReceivedAsync);
        }

    }

}