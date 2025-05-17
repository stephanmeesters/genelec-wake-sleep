use crate::{constants::GNET_TERM, gnet_message::GNetMessage};
use hidapi::{HidDevice, HidError};
use std::io;
use std::{thread::sleep, time::Duration};

pub struct Transport {
    device: HidDevice,
}

impl Transport {
    pub fn new(device: HidDevice) -> Self {
        Transport { device }
    }

    pub fn send(&self, msg: &GNetMessage) -> Result<(), HidError> {
        let data = escape(&msg.message).map_err(|e| HidError::IoError {
            error: io::Error::new(io::ErrorKind::InvalidData, e),
        })?;
        let mut payload = Vec::with_capacity(65);
        payload.push(0x00);
        payload.push(0x80u8.wrapping_add(data.len() as u8));
        payload.extend_from_slice(&data);
        payload.extend(std::iter::repeat_n(0x00, 65 - payload.len()));

        sleep(Duration::from_millis(150));

        self.device.write(&payload)?;
        Ok(())
    }
}

fn escape(msg: &[u8]) -> Result<Vec<u8>, String> {
    if *msg.last().unwrap() != GNET_TERM {
        return Err("Last byte must be GNET_TERM (0x7E)".into());
    }
    let mut escaped = Vec::new();
    for &b in &msg[..msg.len() - 1] {
        match b {
            0x7D => {
                escaped.push(0x7D);
                escaped.push(0x5D);
            }
            0x7E => {
                escaped.push(0x7D);
                escaped.push(0x5E);
            }
            other => escaped.push(other),
        }
    }
    escaped.push(GNET_TERM);
    Ok(escaped)
}
