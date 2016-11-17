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
    protected Random rnd = new Random();

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
        if (message.Text == "reset")
        {
            PromptDialog.Confirm(
                context,
                AfterResetAsync,
                "Are you sure you want to reset the count?",
                "Didn't get that!",
                promptStyle: PromptStyle.Auto);
        }
        else if (message.Text == "start")
        {
             PromptDialog.Number(
                context,
                AfterSetMaximumAsync,
                "Give a positive integer",
                "Didn't get that!",
                promptStyle: PromptStyle.Auto);
            
        }
        else
        {
            await context.PostAsync($"{this.count++}: You said {message.Text}");
            context.Wait(MessageReceivedAsync);
        }
    }

    public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            this.count = 1;
            await context.PostAsync("Reset count.");
        }
        else
        {
            await context.PostAsync("Did not reset count.");
        }
        context.Wait(MessageReceivedAsync);
    }
    
    
    public async Task AfterSetMaximumAsync(IDialogContext context, IAwaitable<long> argument)
    {
        var maxNum = await argument;

        if (maxNum is long)
        {
            this.answer = rnd.Next(maxNum +1) ;
            await context.PostAsync("Random number: "+this.answer);
        }
        else
        {
            await context.PostAsync("Please input an integer.");
        }
        
        context.Wait(MessageReceivedAsync);
    }
}