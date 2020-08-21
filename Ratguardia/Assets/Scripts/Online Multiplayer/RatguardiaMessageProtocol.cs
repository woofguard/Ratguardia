using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// our Trademarked Copyrighted Patented Ratguardia Message Protocol aka RMP(tm)(c)(r) /s
public enum RMP: byte
{
    // card game actions
    Draw = 0x1,      // standalone
    Discard = 0x2,   // followed by index of card to discard
    Steal = 0x3,     // followed by Player
    NoSteal = 0x4,   // followed by Player
    Combatant = 0x5, // followed by index of card sent into battle
    Player = 0x6,    // followed by board index of player
    EndTurn = 0x7,

    // board/deck data
    Deck = 0xA1,     // standalone
    EndDeck = 0xA2,  // standalone
    Card = 0xA3,     // followed by card suit and card name
    
    // player types
    Players = 0xA4,
    Human = 0xA5,    // might have to be followed by name/portrait later
    Network = 0xA6,  // same ^
    RandomAI = 0xA7,
    BasicAI = 0xA8,

    // cards suits
    Chalices = 0xB1,
    Swords = 0xB2,
    Wands = 0xB3,
    Rings = 0xB4,

    // card names
    Assassin = 0xB5,
    Archer = 0xB6,
    Cavalier = 0xB7,
    Jester = 0xB8,
    King = 0xB9,
    Knight = 0xBA,
    Peasant = 0xBB,
    Preyrider = 0xBC,
    Queen = 0xBD,
    Witch = 0xBE,

    // game status/timing stuff?
    QueryPlayers = 0xC1, // client asks server if room is full (standalone)
    NumPlayers = 0xC2,   // server tells if room is full (followed by num players)
    StartGame = 0xC3,

    // player name/portrait info
    Character = 0xD1, // followed by player index, icon, name
    Icon = 0xD2,      // followed by index in portrait array
    Name = 0xD3,      // followed by num chars in name, then by name string in bytes
    EndCharacter = 0xD4
}

// this class doesn't do anything right now, maybe helper functions later or something
public class RatguardiaMessageProtocol : MonoBehaviour
{
    
}
