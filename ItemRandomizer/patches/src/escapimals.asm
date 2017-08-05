arch snes.cpu
lorom

org $83ADA0
	db $FD, $92, $00, $05, $3E, $26, $03, $02, $00, $80, $A2, $B9	//door to parlor
	db $4D, $DE, $40, $00, $00, $00, $01, $00, $00, $80, $00, $00	//door to escape room 1

org $8F98DC
	db $06, $F0 													//setup asm pointer for pre-bt room
	
org $8FF000
	db $A0, $AD, $AC, $AD											//door out pointer for pre-bt
	
org $8FF006
	LDA #$F000														//load door pointer
	STA $7E07B5														//change door out pointer
	JML $8F91BB														//run original code