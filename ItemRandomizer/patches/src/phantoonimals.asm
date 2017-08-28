arch snes.cpu
lorom

org $83ADA0
	db $FD, $92, $00, $05, $3E, $26, $03, $02, $00, $80, $A2, $B9	//door to parlor
	db $13, $CD, $40, $04, $01, $06, $00, $00, $00, $80, $00, $00	//door to bt, changed to ghost
	db $79, $98, $40, $05, $2E, $06, $02, $00, $00, $80, $00, $00	//ghost exit, changed to pre-bt
	
org $8F98DC
	db $06, $F0 													//setup asm pointer for pre-bt room
	
org $8FF000
	db $A0, $AD, $AC, $AD											//door out pointer
	
org $8FF006
	LDA #$0000
	STA $7ED82B														//reset boss flag
	LDA #$F000														//load door pointer
	STA $7E07B5														//change door out pointer
	JML $8F91BB														//run original code
	
org $8FCD3D
	db $18, $F0														//setup asm pointer for ghost
	
org $8FEFF0
	db $B8, $AD														//door out pointer
	
org $8FF018
	LDA $7ED820 													//loads event flags
	BIT #$4000  													//checks for escape flag set
	BEQ quit
	lda #$0000
	sta $7ED8C0
	LDA #$EFF0														//load door pointer
	STA $7E07B5														//change door out pointer
	JML $8FC8D0														//run original code
	
quit:
	JML $8FC8D0														//run original code