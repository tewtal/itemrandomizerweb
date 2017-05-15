arch snes.cpu
lorom

// This skips the intro
org $82eeda
    db $1f

// Hijack init routine to autosave and set door flags
org $828067
    jsl introskip_doorflags

org $80ff00
introskip_doorflags:
    lda $7ed7e2
    bne +
    
    // Set construction zone and red tower elevator doors to blue
    lda $7ed8b6
    ora.w #$0004
    sta $7ed8b6    
    lda $7ed8b2
    ora.w #$0001
    sta $7ed8b2
    
    // Call the save code to create a new file
    lda #$0000
    jsl $818000
+   
    lda #$0000   
    rtl