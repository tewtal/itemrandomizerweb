arch snes.cpu
lorom

// This skips the intro
org $82eeda
    db $1f

// Hijack init routine to autosave and set door flags
org $828067
    jsl introskip_doorflags

// Set morph and missiles room state to always active
org $8fe658
    db $ea, $ea

org $8fe65d
    db $ea, $ea


org $80ff00
introskip_doorflags:
    // Do some checks to see that we're actually starting a new game
    
    // Make sure game mode is 1f
    lda $7e0998
    cmp.w #$001f
    bne .ret
    
    // Check if samus saved energy is 00, if it is, run startup code
    lda $7ed7e2
    bne .ret
    
    // Set construction zone and red tower elevator doors to blue
    lda $7ed8b6
    ora.w #$0004
    sta $7ed8b6    
    lda $7ed8b2
    ora.w #$0001
    sta $7ed8b2
    
    // Set up open mode event bit flags
    lda #$0801
    sta $7ed820
    
    // Call the save code to create a new file
    lda #$0000
    jsl $818000

.ret:   
    lda #$0000
    rtl