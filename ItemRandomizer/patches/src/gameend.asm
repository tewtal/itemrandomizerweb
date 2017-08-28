//door asm of bt/animal room during escape
arch snes.cpu
lorom
org $838BCC
    db $20, $ff

org $8FFF20

LDA $7ED820 //loads event flags
BIT #$4000  //checks for escape flag set
BEQ quit
LDA #$0026
STA $7E0998 //stores gamestate as game end trigger

quit:
RTS