export type CallType = "audio" | "video";
export type CallStatus = "outgoing_calling" | "outgoing_connecting" | "incoming" | "active" | "ended";

export interface ActiveCallState {
  type: CallType;
  status: CallStatus;
  caller: {
    id: string;
    name: string;
    avatarType: string;
    isGroup?: boolean;
    subtitle?: string;
  };
}
