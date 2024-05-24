use std::collections::HashMap;

use bigdecimal::BigDecimal;

fn alphabet(
    message: String
) -> Vec<(char, BigDecimal)> {
    let mut frequency_map = HashMap::new();
    let length: usize = message.chars().count();
    let prec_size: u64 = u64::try_from(length.clone()).unwrap();

    for c in message.chars() {
        let count = frequency_map.entry(c).or_insert(BigDecimal::from(0));
        *count += BigDecimal::from(1);
    }

    let mut res: Vec<_> = frequency_map
        .iter()
        .map(|(char, count)| (*char, count / BigDecimal::from(length as i64).with_prec(prec_size)))
        .collect();

        res.sort_by(|(_, a), (_, b)| a.cmp(b));
    res
}

fn start_interval(alphabet: Vec<(char, BigDecimal)>) -> Vec<(char, BigDecimal, BigDecimal)> {
    let mut interval: Vec<(char, BigDecimal, BigDecimal)> = Vec::new();
    let mut low_boundary: BigDecimal = BigDecimal::from(0);

    for item in &alphabet {
        if item == &alphabet[0] {
            interval.push((alphabet[0].0.clone(), BigDecimal::from(0), alphabet[0].1.clone()));
            low_boundary = alphabet[0].1.clone();
        } else {
            interval.push((item.0.clone(), low_boundary.clone(), low_boundary.clone() + item.1.clone()));
            low_boundary = low_boundary.clone() + item.1.clone();
        }
    }

    interval
}

fn calculate_interval(
    start_interval: Vec<(char, BigDecimal, BigDecimal)>,
    low_boundary: BigDecimal,
    high_boundary: BigDecimal
) -> Vec<(char, BigDecimal, BigDecimal)> {
    let mut interval: Vec<(char, BigDecimal, BigDecimal)> = Vec::new();

    for item in start_interval {
        let new_low_boundary = low_boundary.clone() + (high_boundary.clone() - low_boundary.clone()) * item.1.clone();
        let new_high_boundary = low_boundary.clone() + (high_boundary.clone() - low_boundary.clone()) * item.2.clone();
        interval.push((item.0, new_low_boundary, new_high_boundary));
    }

    interval
}

fn get_char_in_interval(
    value: BigDecimal,
    start_interval: Vec<(char, BigDecimal, BigDecimal)>
) -> (BigDecimal, BigDecimal, char) {
    let mut low_boundary: BigDecimal = BigDecimal::from(0);
    let mut high_boundary: BigDecimal = BigDecimal::from(0);
    let mut ch: char = ' ';

    for item in start_interval {
        if item.1 <= value && value <= item.2 {
            low_boundary = item.1;
            high_boundary = item.2;
            ch = item.0;
        }
    }

    return (low_boundary, high_boundary, ch);
}

fn encode_message(
    message: String,
    start_interval: Vec<(char, BigDecimal, BigDecimal)>
) -> BigDecimal {
    let mut step_interval: Vec<(char, BigDecimal, BigDecimal)> = start_interval.clone();
    let mut i = 0;

    while i < (message.chars().count() - 1) {
        let item = step_interval.iter().find(|item| item.0 == message.chars().nth(i).unwrap()).unwrap();
        let low_boundary: BigDecimal = item.1.clone();
        let high_boundary: BigDecimal = item.2.clone();
        step_interval = calculate_interval(start_interval.clone(), low_boundary, high_boundary);
        i += 1;
    }

    let result = step_interval.iter().find(|item| item.0 == message.chars().last().unwrap()).unwrap();
    result.1.clone()
}

fn decode_message(
    encoded_message: BigDecimal,
    start_interval: Vec<(char, BigDecimal, BigDecimal)>,
    message_length: usize
) -> String {
    let mut decoded_message: String = String::new();
    let mut step_value = encoded_message.clone();
    let mut i = 0;

    while i < message_length {
        let (low_boundary, high_boundary, ch) = get_char_in_interval(step_value.clone(), start_interval.clone());
        step_value = (step_value.clone() - low_boundary.clone()) / (high_boundary.clone() - low_boundary.clone());
        decoded_message.push_str(&ch.to_string());
        i += 1;
    }

    decoded_message
}

fn main() {
    let message: &str = "abaabaaca!";

    let alphabet = alphabet(message.to_string());
    let start_interval = start_interval(alphabet.clone());

    let encoded_message = encode_message(message.to_string(), start_interval.clone());
    let decoded_message = decode_message(encoded_message.clone(), start_interval.clone(), message.chars().count());

    println!("Encoded message - {}", encoded_message);
    println!("Decoded message - {}", decoded_message);
    println!("Are messages same: {}", message == decoded_message.clone());
}