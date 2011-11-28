#ifndef PIECE_H
#define PIECE_H

#include <stdint.h>

#include "side.h"
#include "pinstatus.h"
#include "piecetype.h"
#include "ichessboard.h"


class IPiece
{
public:
    virtual IChessBoard GetBoard() = 0;
	virtual int32_t GetValue() = 0;
    virtual int32_t GetCapValue() = 0;
    virtual Side GetOwner() = 0;
    virtual int32_t GetHomeSquare() = 0;
	virtual void Move(int32_t move)=0;
	virtual void UnMove(int32_t move)=0;
    virtual int GetMoves(int32_t start, int32_t moves[])=0;
	virtual int GetCaptureMoves(int32_t start, int32_t moves[],int32_t square)=0;
	virtual int GetBlockingMoves(int32_t start, int32_t moves[],int32_t ray[], int rayLen)=0;
	virtual int Capture()=0;
	virtual void UnCapture(int32_t square)=0; 
	virtual bool AttackSquare(int32_t square)=0; 
	virtual bool GetDiagonal()=0; 
	virtual bool GetStraight()=0; 
    virtual PinStatus GetPinStatus()=0;
	virtual PieceType GetType()=0;
	virtual void ForceSquare(int32_t square)=0;
	virtual uint64_t GetZKey() = 0;
};



#endif