using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

public class swen221Tests
{

    //Pawn single move
    [Test]
    public void testValid_01()
    {
        string input =
            "a2-a3\n" +
            "";
        string output =
            "8|r|n|b|q|k|b|n|r|\n" +
            "7|p|p|p|p|p|p|p|p|\n" +
            "6|_|_|_|_|_|_|_|_|\n" +
            "5|_|_|_|_|_|_|_|_|\n" +
            "4|_|_|_|_|_|_|_|_|\n" +
            "3|P|_|_|_|_|_|_|_|\n" +
            "2|_|P|P|P|P|P|P|P|\n" +
            "1|R|N|B|Q|K|B|N|R|\n" +
              " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Pawn single move
    [Test]
    public void testValid_02()
    {
        string input =
            "b2-b3\n" +
            "";
        string output =
            "8|r|n|b|q|k|b|n|r|\n" +
            "7|p|p|p|p|p|p|p|p|\n" +
            "6|_|_|_|_|_|_|_|_|\n" +
            "5|_|_|_|_|_|_|_|_|\n" +
            "4|_|_|_|_|_|_|_|_|\n" +
            "3|_|P|_|_|_|_|_|_|\n" +
            "2|P|_|P|P|P|P|P|P|\n" +
            "1|R|N|B|Q|K|B|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Test both black and white move in 1 turn
    [Test]
    public void testValid_04()
    {
        string input =
            "b2-b3 a7-a6\n" +
            "";
        string output =
            "8|r|n|b|q|k|b|n|r|\n" +
            "7|_|p|p|p|p|p|p|p|\n" +
            "6|p|_|_|_|_|_|_|_|\n" +
            "5|_|_|_|_|_|_|_|_|\n" +
            "4|_|_|_|_|_|_|_|_|\n" +
            "3|_|P|_|_|_|_|_|_|\n" +
            "2|P|_|P|P|P|P|P|P|\n" +
            "1|R|N|B|Q|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking that games can have multiple rounds with movement
    [Test]
    public void testValid_05()
    {
        string input =
                "\n" +
            "a2-a3 a7-a6\n" +
            "b2-b3 b7-b6\n" +
            "c2-c3 c7-c6\n" +
            "";
        string output =
            "8|r|n|b|q|k|b|n|r|\n" +
            "7|_|_|_|p|p|p|p|p|\n" +
            "6|p|p|p|_|_|_|_|_|\n" +
            "5|_|_|_|_|_|_|_|_|\n" +
            "4|_|_|_|_|_|_|_|_|\n" +
            "3|P|P|P|_|_|_|_|_|\n" +
            "2|_|_|_|P|P|P|P|P|\n" +
            "1|R|N|B|Q|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking knights can move and jump
    [Test]
    public void testValid_06()
    {
        string input =
            "Nb1-a3 Nb8-a6\n" +
            "";
        string output =
                "8|r|_|b|q|k|b|n|r|\n" +
                "7|p|p|p|p|p|p|p|p|\n" +
                "6|n|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|N|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|P|P|P|P|\n" +
                "1|R|_|B|Q|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking the queenside rooks can move horizontally
    [Test]
    public void testValid_07()
    {
        string input =
            "Nb1-a3 Nb8-a6\n" +
            "Ra1-b1 Ra8-b8\n" +
            "";
        string output =
                "8|_|r|b|q|k|b|n|r|\n" +
                "7|p|p|p|p|p|p|p|p|\n" +
                "6|n|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|N|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|P|P|P|P|\n" +
                "1|_|R|B|Q|K|B|N|R|\n" +
            " w Kk - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking the bishops can move
    [Test]
    public void testValid_08()
    {
        string input =
            "d2-d3 d7-d6\n" +
            "Bc1-f4 Bc8-f5\n" +
            "";
        string output =
                "8|r|n|_|q|k|b|n|r|\n" +
                "7|p|p|p|_|p|p|p|p|\n" +
                "6|_|_|_|p|_|_|_|_|\n" +
                "5|_|_|_|_|_|b|_|_|\n" +
                "4|_|_|_|_|_|B|_|_|\n" +
                "3|_|_|_|P|_|_|_|_|\n" +
                "2|P|P|P|_|P|P|P|P|\n" +
                "1|R|N|_|Q|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking the queenside rooks can move horizontally
    [Test]
    public void testValid_09()
    {
        string input =
            "Ng1-h3 Ng8-h6\n" +
            "Rh1-g1 Rh8-g8\n" +
            "";
        string output =
                "8|r|n|b|q|k|b|r|_|\n" +
                "7|p|p|p|p|p|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|n|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|N|\n" +
                "2|P|P|P|P|P|P|P|P|\n" +
                "1|R|N|B|Q|K|B|R|_|\n" +
            " w Qq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking that queens move diagonally
    [Test]
    public void testValid_12()
    {
        string input =
            "e2-e3 e7-e6\n" +
            "Qd1-g4 Qd8-g5\n" +
            "";
        string output =
                "8|r|n|b|_|k|b|n|r|\n" +
                "7|p|p|p|p|_|p|p|p|\n" +
                "6|_|_|_|_|p|_|_|_|\n" +
                "5|_|_|_|_|_|_|q|_|\n" +
                "4|_|_|_|_|_|_|Q|_|\n" +
                "3|_|_|_|_|P|_|_|_|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|_|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking kings can move
    [Test]
    public void testValid_13()
    {
        string input =
            "e2-e3 e7-e6\n" +
            "Qd1-g4 Qd8-g5\n" +
            "Ke1-d1 Ke8-d8\n" +
            "";
        string output =
                "8|r|n|b|k|_|b|n|r|\n" +
                "7|p|p|p|p|_|p|p|p|\n" +
                "6|_|_|_|_|p|_|_|_|\n" +
                "5|_|_|_|_|_|_|q|_|\n" +
                "4|_|_|_|_|_|_|Q|_|\n" +
                "3|_|_|_|_|P|_|_|_|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|K|_|B|N|R|\n" +
            " w  - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking that pawns can have a double forward move if they have not moved yet
    [Test]
    public void testValid_14()
    {
        string input =
            "d2-d4 d7-d5\n" +
            "";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|p|p|p|_|p|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|p|_|_|_|_|\n" +
                "4|_|_|_|P|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|_|P|P|P|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " w KQkq d6 O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking that white pawns can take pawns
    [Test]
    public void testValid_15()
    {
        string input =
            "d2-d4 e7-e5\n" +
            "d4xe5\n" +
            "";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|p|p|p|p|_|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|P|_|_|_|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|_|P|P|P|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Checking black pawns can take pawns
    [Test]
    public void testValid_16()
    {
        string input =
            "d2-d4 e7-e5\n" +
            "a2-a4 e5xd4\n" +
            "";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|p|p|p|p|_|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|P|_|_|p|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|_|P|P|_|P|P|P|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //White king castling queenside
    [Test]
    public void testValid_17()
    {
        string input =
            "Nb1-a3 d7-d5\n" +
            "d2-d4 a7-a5\n" +
            "Bc1-e3 b7-b5\n" +
            "Qd1-d2 c7-c5\n" +
            "e1-c1\n";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|_|_|_|_|p|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|p|p|p|p|_|_|_|_|\n" +
                "4|_|_|_|P|_|_|_|_|\n" +
                "3|N|_|_|_|B|_|_|_|\n" +
                "2|P|P|P|Q|P|P|P|P|\n" +
                "1|_|_|K|R|_|B|N|R|\n" +
            " b kq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Black king castling queenside
    [Test]
    public void testValid_18()
    {
        string input =
            "d2-d4 Nb8-a6\n" +
            "a2-a4 d7-d5\n" +
            "b2-b4 Bc8-e6\n" +
            "c2-c4 Qd8-d7\n" +
            "e2-e4 e8-c8\n";
        string output =
                "8|_|_|k|r|_|b|n|r|\n" +
                "7|p|p|p|q|p|p|p|p|\n" +
                "6|n|_|_|_|b|_|_|_|\n" +
                "5|_|_|_|p|_|_|_|_|\n" +
                "4|P|P|P|P|P|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|_|_|_|_|_|P|P|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " w KQ - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //White king castling kingside
    [Test]
    public void testValid_19()
    {
        string input =
            "Ng1-h3 d7-d5\n" +
            "e2-e4 a7-a5\n" +
            "Bf1-d3 b7-b5\n" +
            "e1-g1";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|_|_|p|_|p|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|p|p|_|p|_|_|_|_|\n" +
                "4|_|_|_|_|P|_|_|_|\n" +
                "3|_|_|_|B|_|_|_|N|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|Q|_|R|K|_|\n" +
            " b kq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Black king castling kingside
    [Test]
    public void testValid_20()
    {
        string input =
            "a2-a4 Ng8-h6\n" +
            "b2-b4 e7-e5\n" +
            "c2-c4 Bf8-d6\n" +
            "d2-d4 e8-g8\n" +
            "";
        string output =
                "8|r|n|b|q|_|r|k|_|\n" +
                "7|p|p|p|p|_|p|p|p|\n" +
                "6|_|_|_|b|_|_|_|n|\n" +
                "5|_|_|_|_|p|_|_|_|\n" +
                "4|P|P|P|P|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|_|_|_|_|P|P|P|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " w KQ - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Pawn takes black bishop
    [Test]
    public void testValid_21()
    {
        string input =
            "h2-h4 e7-e5\n" +
            "g2-g4 Bf8-a3\n" +
            "b2xBa3";
        string output =
                "8|r|n|b|q|k|_|n|r|\n" +
                "7|p|p|p|p|_|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|p|_|_|_|\n" +
                "4|_|_|_|_|_|_|P|P|\n" +
                "3|P|_|_|_|_|_|_|_|\n" +
                "2|P|_|P|P|P|P|_|_|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }


    //Pawn takes black rook
    [Test]
    public void testValid_22()
    {
        string input =
            "g2-g4 h7-h5\n" +
            "g4-g5 Rh8-h6\n" +
            "g5xRh6";
        string output =
                "8|r|n|b|q|k|b|n|_|\n" +
                "7|p|p|p|p|p|p|p|_|\n" +
                "6|_|_|_|_|_|_|_|P|\n" +
                "5|_|_|_|_|_|_|_|p|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|P|P|_|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " b KQq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Pawn takes white rook
    [Test]
    public void testValid_23()
    {
        string input =
            "h2-h4 g7-g5\n" +
            "Rh1-h3 g5-g4\n" +
            "a2-a4 g4xRh3";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|p|p|p|p|p|p|_|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|P|_|_|_|_|_|_|P|\n" +
                "3|_|_|_|_|_|_|_|p|\n" +
                "2|_|P|P|P|P|P|P|_|\n" +
                "1|R|N|B|Q|K|B|N|_|\n" +
            " w Qkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Pawn takes white bishop
    [Test]
    public void testValid_24()
    {
        string input =
            "e2-e4 e7-e5\n" +
            "Bf1-a6 b7xBa6\n";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|p|_|p|p|_|p|p|p|\n" +
                "6|p|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|p|_|_|_|\n" +
                "4|_|_|_|_|P|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|Q|K|_|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Pawn takes black knight
    [Test]
    public void testValid_25()
    {
        string input =
            "Ng1-h3 a7-a5\n" +
            "Nh3-g5 b7-b5\n" +
            "Ng5-e6 f7xNe6\n";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|_|_|p|p|p|_|p|p|\n" +
                "6|_|_|_|_|p|_|_|_|\n" +
                "5|p|p|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|P|P|P|P|\n" +
                "1|R|N|B|Q|K|B|_|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Pawn takes white knight
    [Test]
    public void testValid_26()
    {
        string input =
            "a2-a4 Ng8-h6\n" +
            "b2-b4 Nh6-g4\n" +
            "c2-c4 Ng4-e3\n" +
            "d2xNe3";
        string output =
                "8|r|n|b|q|k|b|_|r|\n" +
                "7|p|p|p|p|p|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|P|P|P|_|_|_|_|_|\n" +
                "3|_|_|_|_|P|_|_|_|\n" +
                "2|_|_|_|_|P|P|P|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Black pawn takes white queen
    [Test]
    public void testValid_27()
    {
        string input =
            "e2-e4 a7-a5\n" +
            "Qd1-h5 b7-b5\n" +
            "Qh5-h6 g7xQh6\n";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|_|_|p|p|p|p|_|p|\n" +
                "6|_|_|_|_|_|_|_|p|\n" +
                "5|p|p|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|P|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|_|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Pawn takes white queen
    [Test]
    public void testValid_28()
    {
        string input =
            "a2-a4 e7-e5\n" +
            "b2-b4 Qd8-h4\n" +
            "c2-c4 Qh4-h3\n" +
            "g2xQh3\n";
        string output =
                "8|r|n|b|_|k|b|n|r|\n" +
                "7|p|p|p|p|_|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|p|_|_|_|\n" +
                "4|P|P|P|_|_|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|P|\n" +
                "2|_|_|_|P|P|P|_|P|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //En passant black takes white pawn
    [Test]
    public void testValid_29()
    {
        string input =
            "h2-h4 a7-a5\n" +
            "g2-g4 a5-a4\n" +
            "b2-b4 a4xb3\n";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|_|p|p|p|p|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|_|_|P|P|\n" +
                "3|_|p|_|_|_|_|_|_|\n" +
                "2|P|_|P|P|P|P|_|_|\n" +
                "1|R|N|B|Q|K|B|N|R|\n" +
            " w KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Take pawn with queen
    [Test]
    public void testValid_30()
    {
        string input = "e2-e4 a7-a5\n" +
                "Qd1-h5 b7-b5\n" +
                "Qh5xh7\n";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|_|_|p|p|p|p|p|Q|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|p|p|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|P|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|_|K|B|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Queen move horizontally
    [Test]
    public void testValid_31()
    {
        string input = "e2-e4 a7-a5\n" +
                    "Qd1-f3 b7-b5\n" +
                    "Qf3-h3\n";
        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|_|_|p|p|p|p|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|p|p|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|P|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|Q|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|_|K|B|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Bishop move diagonally, taking with bishop
    [Test]
    public void testValid_32()
    {
        string input = "e2-e4 h7-h5\n" +
    "Bf1-a6 g7-g5\n" +
                "Ba6xb7\n";

        string output =
                "8|r|n|b|q|k|b|n|r|\n" +
                "7|p|B|p|p|p|p|_|_|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|_|_|_|_|_|_|p|p|\n" +
                "4|_|_|_|_|P|_|_|_|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|P|P|_|P|P|P|\n" +
                "1|R|N|B|Q|K|_|N|R|\n" +
            " b KQkq - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }



    //Testing many combinations of moves
    //This was within game003.txt
    [Test]
    public void testValid_33()
    {
        string input = "d2-d4 e7-e5\n"
                + "e2-e3 d7-d5\n"
                + "c2-c4 d5xc4\n"
                + "Bf1xc4 Nb8-c6\n"
                + "Ng1-f3 e5-e4\n"
                + "Nf3-d2 f7-f5\n"
                + "Bc4xNg8 Rh8xBg8\n"
                + "e1-g1 Bc8-e6\n"
                + "Qd1-h5 g7-g6\n"
                + "Qh5xh7 f5-f4\n"
                + "e3xf4 Nc6xd4\n"
                + "Nb1-c3 Bf8-d6\n"
                + "Nd2xe4 Bd6xf4\n"
                + "g2-g3 Bf4xg3\n"
                + "h2xBg3 Nd4-f3\n"
                + "Kg1-g2 Nf3-h4\n"
                + "Qh7xNh4 Qd8xQh4\n"
                + "g3xQh4 g6-g5\n"
                + "Ne4-f6 Ke8-e7\n"
                + "Nf6xRg8 Ra8xNg8\n"
                + "h4-h5 g5-g4\n"
                + "h5-h6 Rg8-h8\n"
                + "Rf1-h1 Ke7-f7\n"
                + "Nc3-e4 Rh8-h7\n"
                + "Ne4-g5 Kf7-g6\n"
                + "Ng5xRh7 Kg6xNh7\n"
                + "";

        string output =
                  "8|_|_|_|_|_|_|_|_|\n"
                + "7|p|p|p|_|_|_|_|k|\n"
                + "6|_|_|_|_|b|_|_|P|\n"
                + "5|_|_|_|_|_|_|_|_|\n"
                + "4|_|_|_|_|_|_|p|_|\n"
                + "3|_|_|_|_|_|_|_|_|\n"
                + "2|P|P|_|_|_|P|K|_|\n"
                + "1|R|_|B|_|_|_|_|R|\n"
            + " w  - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Testing game002
    //Testing checkmate moves work
    [Test]
    public void testValid_42()
    {
        string input =
            "f2-f3 e7-e5\n"
            + "g2-g4 Qd8-h4";
        string output =
                "8|r|n|b|_|k|b|n|r|\n"
                + "7|p|p|p|p|_|p|p|p|\n"
                + "6|_|_|_|_|_|_|_|_|\n"
                + "5|_|_|_|_|p|_|_|_|\n"
                + "4|_|_|_|_|_|_|P|q|\n"
                + "3|_|_|_|_|_|P|_|_|\n"
                + "2|P|P|P|P|P|_|_|P|\n"
                + "1|R|N|B|Q|K|B|N|R|\n" +
             " w KQkq - B";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }

    //Testing castling into check works
    [Test]
    public void testValid43()
    {
        string input =
                "a2-a4 f7-f5\n" +
                "Ra1-a3 a7-a5\n" +
                "Ra3-f3 Ra8-a7\n" +
                "Rf3xf5 Ra7-a8\n" +
                "Rf5xBf8 Ke8xRf8\n" +
                "g2-g4 Ra8-a6\n" +
                "Ng1-f3 Ra6-f6\n" +
                "b2-b3 Rf6xNf3\n" +
                "b3-b4 Rf3xf2\n" +
                "b4-b5 Rf2-f5\n" +
                "Bf1-h3 Rf5-c5\n" +
                "e1-g1\n";

        string output =
                "8|_|n|b|q|_|k|n|r|\n" +
                "7|_|p|p|p|p|_|p|p|\n" +
                "6|_|_|_|_|_|_|_|_|\n" +
                "5|p|P|r|_|_|_|_|_|\n" +
                "4|P|_|_|_|_|_|P|_|\n" +
                "3|_|_|_|_|_|_|_|B|\n" +
                "2|_|_|P|P|P|_|_|P|\n" +
                "1|_|N|B|Q|_|R|K|_|\n" +
             " b  - O";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(output));
    }


    //Testing stalemating works
    [Test]
    public void testValid_44()
    {
        string input =
                "h2-h4 a7-a5\n" +
                "c2-c4 h7-h5\n" +
                "Qd1-a4 Ra8-a6\n" +
                "Qa4xa5 Ra6-h6\n" +
                "Qa5xc7 f7-f6\n" +
                "Qc7xd7 Ke8-f7\n" +
                "Qd7xb7 Qd8-d3\n" +
                "Qb7xNb8 Qd3-h7\n" +
                "Qb8xBc8 Kf7-g6\n" +
                "Qc8-e6" +
                "";
        string board =
                "8|_|_|_|_|_|b|n|r|\n" +
                "7|_|_|_|_|p|_|p|q|\n" +
                "6|_|_|_|_|Q|p|k|r|\n" +
                "5|_|_|_|_|_|_|_|p|\n" +
                "4|_|_|P|_|_|_|_|P|\n" +
                "3|_|_|_|_|_|_|_|_|\n" +
                "2|P|P|_|P|P|P|P|_|\n" +
                "1|R|N|B|_|K|B|N|R|\n" +
             " b KQ - S";

        bStateTests.runMoves(coor1(input), coor2(input), convertBoard(board));
    }

    //Promotion to queen and knight
    [Test]
    public void testValid_34()
    {
        List<string> input = new List<string>{ "b2-b4", "g7-g5",
                "b4-b5","g5-g4",
                "b5-b6","g4-g3",
                "h2xg3","a7xb6",
                "Nb1-a3","Ng8-h6",
                "g3-g4","b6-b5",
                "g4-g5","b5-b4",
                "g5-g6","b4-b3",
                "g6-g7","b3-b2",
                "g7-g8=N","b2-b1=Q"};

        string output =
                "8|r|n|b|q|k|b|N|r|\n" +
                "7|_|p|p|p|p|p|_|p|\n" +
                "6|_|_|_|_|_|_|_|n|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|N|_|_|_|_|_|_|_|\n" +
                "2|P|_|P|P|P|P|P|_|\n" +
                "1|R|q|B|Q|K|B|N|R|\n" +
             " w KQkq - O";

        runMoves(input, convertBoard(output));
    }

    //Promotion to rook and bishop
    [Test]
    public void testValid_35()
    {
        List<string> input = new List<string>{"b2-b4","g7-g5",
                "b4-b5","g5-g4",
                "b5-b6","g4-g3",
                "h2xg3","a7xb6",
                "Nb1-a3","Ng8-h6",
                "g3-g4","b6-b5",
                "g4-g5","b5-b4",
                "g5-g6","b4-b3",
                "g6-g7","b3-b2",
                "g7-g8=R","b2-b1=B"};

        string output =
                "8|r|n|b|q|k|b|R|r|\n" +
                "7|_|p|p|p|p|p|_|p|\n" +
                "6|_|_|_|_|_|_|_|n|\n" +
                "5|_|_|_|_|_|_|_|_|\n" +
                "4|_|_|_|_|_|_|_|_|\n" +
                "3|N|_|_|_|_|_|_|_|\n" +
                "2|P|_|P|P|P|P|P|_|\n" +
                "1|R|b|B|Q|K|B|N|R|\n" +
             " w KQkq - O";

        runMoves(input, convertBoard(output));
    }

    private static Regex boardNumberRemoval = new Regex(@"[\d]\|");
    private static Regex moveSplitter = new Regex(@"-|x|=");
    private static Regex spaceOrLine = new Regex(@"\s|\n");

    /**Converts a board string from the format used in swen221 to the format currently used */
    private static string convertBoard(string board)
    {
        board = boardNumberRemoval.Replace(board, string.Empty);
        board = board.Replace("\n", "\r\n");
        board = board.Replace("|", "");
        return board;
    }

    /**Gets the first coordinate from a move string */
    private static Coordinate firstCoordinate(string moveString)
    {
        string s = moveSplitter.Split(moveString)[0];
        s = s.Substring(s.Length - 2);
        return new Coordinate(s);
    }

    /**Gets the 2nd coordinate from a move string */
    private static Coordinate secondCoordinate(string moveString)
    {
        string s = moveSplitter.Split(moveString)[1];
        s = s.Substring(s.Length - 2);
        return new Coordinate(s);
    }

    /**Gets the starting coordinates from the series of move strings */
    private static List<Coordinate> coor1(string moveStrings)
    {
        List<Coordinate> coors = new List<Coordinate>();
        foreach (string move in spaceOrLine.Split(moveStrings))
        {
            if (move.Equals(string.Empty)) { continue; }
            coors.Add(firstCoordinate(move));
        }
        return coors;
    }

    /**Gets the ending coordinate from the move string */
    private static List<Coordinate> coor2(string moveStrings)
    {
        List<Coordinate> coors = new List<Coordinate>();
        foreach (string move in spaceOrLine.Split(moveStrings))
        {
            if (move.Equals(string.Empty)) { continue; }
            coors.Add(secondCoordinate(move));
        }
        return coors;
    }

//TODO check all your editmode tests still run after your changes of adding in Hands is finished

    /**Takes in moves as a String in algebraic notation
Checks that the move specified is one of the offered moves, allows for promotion moves, 
AND checks that the board goes into check at the correct times, also checking the end result board is correctly
specified.
Portable Game Notation:
a4-d3    These show the starting and end position of a piece
Letters before the first coordinate show what type of piece is moved, No letter is pawn. getType can tell you the type
Na4-b3 is moving a knight from a4 to b3
a4-b3+ the + shows the other team ends in check
a4xb3   the x shows a piece was taken at b3
a4xNb3 the N shows the type of piece taken
a3-a4=R  the =R shows promotion to Rook
I am NOT using ep to show en passant, or O-O-O to show castling, or # to show checkmate */
    public static void runMoves(List<string> moves, string board)
    {
        BoardState bState = bStateTests.makeBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - O");
        for (int i = 0; i < moves.Count; i++)
        {
            string moveString = moves[i];
            Coordinate from = firstCoordinate(moveString);
            Coordinate to = secondCoordinate(moveString);
            //Look through all the valid moves, find one which the From and To match.
            Move move = null;
            List<Move> allMoves = Processing.allValidMoves(bState, bState.currentTeam());
            foreach (Move mo in allMoves)
            {
                if(!(mo is PieceMove)){ continue; }
                PieceMove m = (PieceMove)mo;
                //Check this move has correct from and to coords
                if (!m.getFrom().Equals(from) || !m.getTo().Equals(to))
                { continue; }
                //Check the type of piece moved matches what we expect
                if (!(isRightType(moveSplitter.Split(moveString)[0], 
				m.getPiece(bState)))) { continue; }
                //Simulate the move
                BoardState cloneState = bState.clone();
                Piece killedPiece = Processing.simulateMove(cloneState, m);
                //If its a killing move, check the piece killed is correct type
                if (moveString.Contains("x"))
                {
                    if (killedPiece == null || !(isRightType(moveSplitter.Split(moveString)[1], killedPiece)))
                    {
                        continue;
                    }
                }
                else //If its not a killing move, check no piece is killed
                {
                    if (killedPiece != null) { continue; }
                }
                //If its a promotoion move, check the promotion is to the correct type
                if (moveString.Contains("="))
                {
                    if (!(m is PromoteMove) || !(isRightType(moveSplitter.Split(moveString)[2], ((PromoteMove)m).promotedTo)))
                    {
                        continue;
                    }
                }
                //If its a checking move, ensure the board becomes checked
                bool inCheck = Processing.inCheck(cloneState, bState.currentTeam().nextTeam());
                if (moveString.Contains("+"))
                {
                    if (!inCheck) { continue; }
                }
                else //If its not a checking move, ensure the board does not become checked
                {
                    if (inCheck) { continue; }
                }
                move = m;
                break;
            }
            if (move == null)
            {
                Assert.Fail("Move " + moveString + " not valid");
            }
            Assert.AreEqual(bState.getPiece(from).getTeam(), bState.currentTeam());
            move.doMoveState(bState);
            Processing.updateGameResult(bState, bState.currentTeam().nextTeam());
            bState.setTeam(bState.currentTeam().nextTeam());
        }
        Assert.AreEqual(board, bState.ToString());
    }

    /**Checks if a Piece is the right type, based on the section of move string passed in (foo) */
    private static bool isRightType(string foo, Piece p)
    {
        if (foo.Contains("K"))
        {
            return p is King;
        }
        else if (foo.Contains("Q"))
        {
            return p is Queen;
        }
        else if (foo.Contains("N"))
        {
            return p is Knight;
        }
        else if (foo.Contains("B"))
        {
            return p is Bishop;
        }
        else if (foo.Contains("R"))
        {
            return p is Rook;
        }
        else
        {
            return p is Pawn;
        }
    }
}