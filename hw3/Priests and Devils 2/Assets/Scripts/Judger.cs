using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class Judger : System.Object {
    private static Judger _instance; 
    public static Judger getInstance() { //使用单例模式
        if (_instance == null) _instance = new Judger ();
        return _instance;
    }
    public int check(CoastCon fromCoast,CoastCon toCoast,BoatCon boat) {	// 0->not finish, 1->lose, 2->win
        int fromP = 0, fromD = 0, toP = 0, toD = 0;
        int[] fromCount = fromCoast.getCharacterNum();
        fromP += fromCount[0];
        fromD += fromCount[1];

        int[] toCount = toCoast.getCharacterNum ();
        toP += toCount[0];
        toD += toCount[1];

        if (toP + toD == 6) return 2; //win
        int[] boatCount = boat.getCharacterNum ();
        if (boat.getStatus () == -1) {
            toP += boatCount[0]; toD += boatCount[1];	// boat at toCoast
        }
        else {
            fromP += boatCount[0]; fromD += boatCount[1];	// boat at fromCoast	
        }
        if (fromP < fromD && fromP > 0) return 1; //lose		
        if (toP < toD && toP > 0) return 1; //lose
        return 0;			// not finish
    }
}