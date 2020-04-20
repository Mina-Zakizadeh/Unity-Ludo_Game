using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AIPlayer_UtilyAI :AIPlayer
    {
    override protected PlayerStone PickStoneToMove(PlayerStone[] legalStones)
    {
        Debug.log("PickStoneToMove");
        return base.PickStoneToMove(legalStones);

    }
}

