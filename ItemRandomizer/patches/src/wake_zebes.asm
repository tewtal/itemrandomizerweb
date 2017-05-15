arch snes.cpu
lorom

// Door ASM pointer (Door into small corridor before construction zone)
org $838eb4
    db $00, $ff

// Door ASM to set Zebes awake
org $8fff00
    lda.w $7ed820
    ora.w $0001
    sta.w $7ed820
    rts