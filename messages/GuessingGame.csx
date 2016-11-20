using System;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class GuessingGame
{

    private static Random rnd = new Random();
    private int answer;
    public int NumOfGuess { get; private set; }
    public bool IsWin { get; private set; }

    public GuessingGame(int upperBound)
    {   
         this.answer = rnd.Next(1, (int) upperBound+1) ;
    }


    public Result Guess(int guess)
    {
        NumOfGuess++;
        if (answer == guess)
        {
            IsWin = true;
            return Result.Correct;
        }
        else if (guess < 0)
        {
            return Result.Quit;
        }
        
        return answer < guess ? Result.TooHigh : Result.TooLow; 
    }
}

[Serializable]
public enum Result
{
    Correct,
    TooHigh,
    TooLow,
    Quit,
}