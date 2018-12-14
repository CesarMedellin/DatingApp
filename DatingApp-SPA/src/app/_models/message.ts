export interface Message {
    id: number;
    senderId: number;
    senderKnownAs: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientKnownAs: string;
    reipientPhotoUr: string;
    content: string;
    isRead: boolean;
    dateRead: Date;
    massegaeSent: Date;

}