using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Deck
{
    private static System.Random random = new System.Random();
    public static int[] DefaultCards = { 30, 30 };

    private Team team;
    private Dictionary<int, int> cardCount; //this is an array, linking from card number (1,2,3,etc) 
    //to how many are in the deck (total of 60 cards in the deck)

    public Deck(Team team, int[] deckCards)
    {
        this.team = team;
        cardCount = new Dictionary<int, int>();
        int sum = 0;
        for (int i = 0; i < deckCards.Length; i++)
        {
            sum += deckCards[i];
            if (deckCards[i] > 0) { cardCount.Add(i + 1, deckCards[i]); }
            if (deckCards[i] < 0) { throw new System.Exception("Can't have negative numebrs of cards in a deck"); }
        }
        if (sum != 60) { throw new System.Exception("You need 60 cards in a deck"); }
    }

    public Card draw()
    {
        float cardsSoFar = 0;
        double f = random.NextDouble();
        foreach (int cardNum in cardCount.Keys)
        {
            cardsSoFar += cardCount[cardNum];
            if (f < cardsSoFar / 60.0)
            {
                return this.getCard(cardNum);
            }
        }
        throw new System.Exception("Something went wrong with drawing cards");
    }

    public Card getCard(int cardNum)
    {
        switch (cardNum)
        {
            case 1:
                return new Rookify(this.team);
            case 2:
                return new SummonPawn(this.team);
            default:
                throw new System.Exception("a card was asked for that has not been added to getCard yet");
        }
    }

    /**A cloning method for Decks */
    public Deck clone()
    {
        int maxValueKey = cardCount.Aggregate((x, y) => x.Key > y.Key ? x : y).Key;
        int[] deckCards = new int[maxValueKey];
        foreach (int key in cardCount.Keys)
        {
            deckCards[key - 1] = cardCount[key];
        }
        return new Deck(this.team, deckCards);
    }
}

